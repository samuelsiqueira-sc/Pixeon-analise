// Nome da entidade: Linha de Cotação
if (typeof (QuoteDetail) === undefined || typeof (QuoteDetail) === "undefined") { QuoteDetail = {}; }

var QuoteDetail_LogicalName = "quotedetail";
QuoteDetail.Functions = {

    OnLoad: function (executionContext) {
        "use strict";
        QuoteDetail.Functions.setPrincipalContract(executionContext);
    },

    OnSave: function (executionContext) {
        "use strict";
        QuoteDetail.Functions.VerifyBillingMethod(executionContext);
    },

    ContractLine_OnChange: function (executionContext) {
        "use strict";
        QuoteDetail.Functions.VerifyBillingMethod(executionContext);
    },

    //ExtendedPSA - Tasks 2519, 2538. Verificar as regras de negócio da Task 2518. - Leandro
    VerifyBillingMethod: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        var salesOrderDetailLookUp = formContext.getAttribute("smt_lp_contract_line").getValue();

        if (formContext.getAttribute("smt_lp_contract_line").getValue() !== null) {
            salesOrderDetailLookUp = formContext.getAttribute("smt_lp_contract_line").getValue()[0];
            if (formContext.getAttribute("msdyn_billingmethod").getValue() === null) {
                var messageEmptyBillingMethod = MultipleLanguageHelp.returnMessage("QuoDetail_EmptyBillingMethod", "ExtendedPSA");
                formContext.ui.setFormNotification(messageEmptyBillingMethod, "WARNING", "NotEmptyBillingMethod");
                QuoteDetail.Functions.showAlertDialog(messageEmptyBillingMethod);
                formContext.getAttribute("smt_lp_contract_line").setValue(null);
                QuoteDetail.Functions.PreventSave(executionContext);
            }
            else {
                formContext.ui.clearFormNotification("NotEmptyBillingMethod");
                var salesOrderDetailRecord = QuoteDetail.Functions.RetrieveSalesOrderDetail(salesOrderDetailLookUp.id, formContext);
            }
        }
    },

    RetrieveSalesOrderDetail: function (salesorderdetailId, formContext) {
        "use strict";
        var salesorderdetail = null;
        var globalContext = Xrm.Utility.getGlobalContext();
        salesorderdetailId = salesorderdetailId.replace("{", "").replace("}", "");

        Xrm.WebApi.online.retrieveRecord("salesorderdetail", salesorderdetailId, "?$select=msdyn_billingmethod").then(
            function success(result) {
                var msdyn_billingmethod = result["msdyn_billingmethod"];
                var msdyn_billingmethod_formatted = result["msdyn_billingmethod@OData.Community.Display.V1.FormattedValue"];

                if (msdyn_billingmethod !== undefined && msdyn_billingmethod !== null) {
                    if (msdyn_billingmethod !== formContext.getAttribute("msdyn_billingmethod").getValue()) {
                        var messageSameBillingMethod = MultipleLanguageHelp.returnMessage("QuoDetail_SameBillingMethod", "ExtendedPSA");
                        formContext.ui.setFormNotification(messageSameBillingMethod, "WARNING", "NotSameBillingMethod");
                        QuoteDetail.Functions.showAlertDialog(messageSameBillingMethod);
                        formContext.getAttribute("smt_lp_contract_line").setValue(null);
                        QuoteDetail.Functions.PreventSave(executionContext);
                    }
                    else {
                        formContext.ui.clearFormNotification("NotSameBillingMethod");
                    }
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    PreventSave: function (executionContext) {
        "use strict";
        if (executionContext !== null && executionContext.getEventArgs() !== null) {
            var eventArgs = executionContext.getEventArgs();
            eventArgs.preventDefault();
        }
    },

    setPrincipalContract: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        if (formContext.ui.getFormType() === 1) {
            var quote = formContext.getControl("quoteid");

            if (quote.getAttribute().getValue() !== null) {
                var contract = QuoteDetail.Functions.getContract(quote, formContext);
            }
        }
    },

    getContract: function (quote, formContext) {
        "use strict";
        var id = quote.getAttribute().getValue()[0].id.replace("{", "").replace("}", "");
        var result = { data: null };
        var globalContext = Xrm.Utility.getGlobalContext();

        Xrm.WebApi.online.retrieveRecord("quote", id, "?$select=_smt_lp_contract_main_value,smt_pl_additive_contract").then(
            function success(response) {
                result.data = {
                    _smt_lp_contract_main_value: response["_smt_lp_contract_main_value"],
                    _smt_lp_contract_main_value_formatted: response["_smt_lp_contract_main_value@OData.Community.Display.V1.FormattedValue"],
                    _smt_lp_contract_main_value_lookuplogicalname: response["_smt_lp_contract_main_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                    smt_pl_additive_contract: response["smt_pl_additive_contract"],
                    smt_pl_additive_contract_formatted: response["smt_pl_additive_contract@OData.Community.Display.V1.FormattedValue"]
                }

                if (result !== null && result.data !== null) {
                    var isAdditive = result.data.smt_pl_additive_contract;
                    if (isAdditive === 100000000 || isAdditive === "100000000") {
                        var lookup = new Array();
                        lookup[0] = new Object();
                        lookup[0].id = result.data._smt_lp_contract_main_value;
                        lookup[0].name = result.data._smt_lp_contract_main_value_formatted;
                        lookup[0].entityType = "salesorder";

                        formContext.getAttribute("smt_lp_main_contract").setValue(lookup);
                    }
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    showAlertDialog: function (message_) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: message_ };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {

            },
            function (error) {

            })
    }

}
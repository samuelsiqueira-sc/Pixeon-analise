if (typeof (ChangeRequest) === undefined || typeof (ChangeRequest) === "undefined") { ChangeRequest = {}; }

var ChangeRequest_LogicalName = "smt_change_request";
ChangeRequest.Functions = {

    OnLoad: function (_executionContext) {
        "use strict";
        var smartHelper = new SmartHelper(_executionContext);
    },

    OnSave: function (_executionContext) {
        "use strict";
        var smartHelper = new SmartHelper(_executionContext);
    },

    CustomFunction: function (smartHelper) {
        "use strict";
    },

    verifyQuoteContractHasValue: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        formContext.ui.clearFormNotification("notFound");
        formContext.ui.clearFormNotification("notFoundContract");

        var quote = formContext.getControl("smt_lp_quote");
        var contract = formContext.getControl("smt_lp_contract_main");

        if (quote !== null && contract !== null) {
            if (quote.getAttribute().getValue() !== null) {
                quote.setDisabled(true);
                contract.setDisabled(true);
            }
            else {
                quote.setDisabled(false);
                contract.setDisabled(false);
            }
        }
        else {
            formContext.ui.setFormNotification("Não foi possível encontrar a cotação ou contrato no formulário, favor tentar novamente ou contatar o administrador", "WARNING", "notFound");
        }
    },

    setContract: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        formContext.ui.clearFormNotification("notFound");
        formContext.ui.clearFormNotification("notFoundContract");
        //MUDAR PRA ORDEM
        var quote = formContext.getControl("smt_lp_quote");
        var contractValue = formContext.getControl("smt_lp_contract_main");
        if (quote !== null && contractValue !== null) {
            if (quote.getAttribute().getValue() !== null) {
                var contractId = ChangeRequest.Functions.getContractFromQuote(quote, formContext);
            }
            else {
                contractValue.getAttribute().setValue(null);
            }
        }
        else {
            formContext.ui.setFormNotification("Não foi possível encontrar a cotação ou contrato no formulário, favor tentar novamente ou contatar o administrador", "WARNING", "notFound");
        }
    },

    getContractFromQuote: function (quote, formContext) {
        "use strict";
        var globalContext = Xrm.Utility.getGlobalContext();

        var result = { isFault: false, data: null, erro: null };

        Xrm.WebApi.online.retrieveRecord("quote", quote.getAttribute().getValue()[0].id.replace("{", "").replace("}", "") , "?$select=_smt_lp_contract_main_value").then(
            function success(response) {
                    result.data = {
                        id: response["_smt_lp_contract_main_value"],
                        name: response["_smt_lp_contract_main_value@OData.Community.Display.V1.FormattedValue"],
                        logicalName: response["_smt_lp_contract_main_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
                };

                if (result !== null && result !== undefined && result.data !== null && result.data !== undefined && result.data.id !== null) {
                    var lookup = new Array();
                    lookup[0] = new Object();
                    lookup[0].id = result.data.id;
                    lookup[0].name = result.data.name;
                    lookup[0].entityType = "salesorder";

                    formContext.getAttribute("smt_lp_contract_main").setValue(lookup);
                }
                else {
                    formContext.ui.setFormNotification("Não foi possível encontrar o contrato da cotação especifica, favor tentar novamente", "WARNING", "notFoundContract");
                    quote.getAttribute().setValue(null);
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    }

}
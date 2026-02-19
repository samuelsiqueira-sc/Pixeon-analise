if (typeof (PurchasingPeriod) === undefined || typeof (PurchasingPeriod) === "undefined") { PurchasingPeriod = {}; }

var PurchasingPeriod_LogicalName = "smt_purchasing_period";
PurchasingPeriod.Functions = {
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

    FinishPurchasingPeriod: function (primaryControl) {
        "use strict";
        // debugger;
        var id = primaryControl.data.entity.getId().replace("{", "").replace("}", "");

        var confirmStrings = {
            text: "",
            title: "Confirmação de finalização",
            subtitle: "Deseja realmente encerrar esse Período Aquisitivo?",
            "cancelButtonLabel": "Não",
            confirmButtonLabel: "Sim"
        };
        var confirmOptions = { height: 100, width: 400 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    primaryControl.getControl("smt_st_closing_reason").setVisible(true);
                    primaryControl.getAttribute("smt_st_closing_reason").setRequiredLevel("required");
                    var paiddays = 0;

                    if (primaryControl.getAttribute("smt_st_closing_reason").getValue() === null || undefined) {
                        var message = "Não é possível encerrar o período aquisitivo sem informar a razão do encerramento";
                        PurchasingPeriod.Functions.showAlertDialog(message);
                    }
                    else {

                        // Se o statuscode for "em aquisição"
                        if (primaryControl.getAttribute("statuscode").getValue() === 1) {

                            // O campo dias pagos vai receber ("contagem dos dias" - "dias utilizados")
                            var contdays = primaryControl.getAttribute("smt_dc_quantity_days").getValue();
                            var useddays = primaryControl.getAttribute("smt_int_useddays_period").getValue();
                            paiddays = contdays - useddays;

                            if (paiddays < 0)
                                paiddays = 0;
                        }
                        // Se o statuscode for "adquirido"
                        else if (primaryControl.getAttribute("statuscode").getValue() === 100000000) {
                            // O campo dias pagos vai receber o valor que está em "saldo de dias"
                            var qtd = primaryControl.getAttribute("smt_int_quantity").getValue();
                            var daysUsed = primaryControl.getAttribute("smt_int_useddays_period").getValue();
                            paiddays = qtd - daysUsed;
                        }

                        var parameters = {};
                        var entity = {};
                        entity.id = id;
                        entity.entityType = "smt_purchasing_period";
                        parameters.entity = entity;
                        parameters.DiasPagos = paiddays;

                        var smt_AC_Finish_PurchasingPeriodRequest = {
                            entity: parameters.entity,
                            DiasPagos: parameters.DiasPagos,

                            getMetadata: function () {
                                return {
                                    boundParameter: "entity",
                                    parameterTypes: {
                                        "entity": {
                                            "typeName": "mscrm.smt_purchasing_period",
                                            "structuralProperty": 5
                                        },
                                        "DiasPagos": {
                                            "typeName": "Edm.Decimal",
                                            "structuralProperty": 1
                                        }
                                    },
                                    operationType: 0,
                                    operationName: "smt_AC_Finish_PurchasingPeriod"
                                };
                            }
                        };

                        Xrm.WebApi.online.execute(smt_AC_Finish_PurchasingPeriodRequest).then(
                            function success(result) {
                                if (result.ok) {
                                    primaryControl.data.refresh();
                                }
                            },
                            function (error) {
                                Xrm.Navigation.openAlertDialog(error.message);
                            }
                        );
                        //var req = new XMLHttpRequest();
                        //req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_purchasing_periods(" + id + ")/Microsoft.Dynamics.CRM.smt_AC_Finish_PurchasingPeriod", false);
                        //req.setRequestHeader("OData-MaxVersion", "4.0");
                        //req.setRequestHeader("OData-Version", "4.0");
                        //req.setRequestHeader("Accept", "application/json");
                        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                        //req.onreadystatechange = function () {
                        //    if (this.readyState === 4) {
                        //        req.onreadystatechange = null;
                        //        if (this.status === 204) {
                        //            //Success - No Return Data - Do Something
                        //            primaryControl.data.refresh();
                        //        } else {
                        //            Xrm.Utility.alertDialog(this.statusText);
                        //        }
                        //    }
                        //};
                        //req.send(JSON.stringify(parameters));
                    }
                }
            }
        );
    },

    setAcumulateDays: function (primaryControl) {
        //debugger;
        "use strict";
        var formcontext = primaryControl.getFormContext();
        var formType = formcontext.ui.getFormType();

        if ((formcontext.getAttribute("smt_lp_resource").getValue() !== null && formType === 1) || (formcontext.getAttribute("smt_lp_resource").getValue() !== null && formcontext.getAttribute("smt_lp_modelcontract").getValue() === null)) {

            var resourceId = formcontext.getAttribute("smt_lp_resource").getValue()[0].id;
            resourceId = resourceId.replace("{", "").replace("}", "");

            var recordId = formcontext.data.entity.getId();
            recordId = recordId.replace("{", "").replace("}", "");

            var contractmodel = PurchasingPeriod.Functions.getModelContract(resourceId, formcontext);
            //results = PurchasingPeriod.Functions.getPurchasePeriod(resourceId);

            if (contractmodel !== null && contractmodel !== undefined) {
                //debugger;
                var lpmodel = new Array();
                lpmodel[0] = new Object();
                lpmodel[0].id = contractmodel.data._smt_lp_model_contract_value;
                lpmodel[0].name = contractmodel.data._smt_lp_model_contract_value_formatted;
                lpmodel[0].entityType = contractmodel.data._smt_lp_model_contract_value_lookuplogicalname;
                formcontext.getAttribute("smt_lp_modelcontract").setValue(lpmodel);

            } else {
                return;
            }
        }

    },

    getModelContract: function (resourceId, formContext) {
        // debugger;
        "use strict";
        var globalContext = Xrm.Utility.getGlobalContext();
        var contracmodelId = "";
        var result = null;

        //var req = new XMLHttpRequest();
        //req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/bookableresources(" + resourceId + ")?$select=_smt_lp_model_contract_value", false);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 200) {
        //            var response = JSON.parse(this.response);
        //            results.data = {
        //                _smt_lp_model_contract_value: response["_smt_lp_model_contract_value"],
        //                _smt_lp_model_contract_value_formatted: response["_smt_lp_model_contract_value@OData.Community.Display.V1.FormattedValue"],
        //                _smt_lp_model_contract_value_lookuplogicalname: response["_smt_lp_model_contract_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
        //            };
        //        } else {
        //            PurchasingPeriod.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();
        Xrm.WebApi.online.retrieveRecord("bookableresource", resourceId, "?$select=_smt_lp_model_contract_value").then(
            function success(result) {
                var _smt_lp_model_contract_value = result["_smt_lp_model_contract_value"];
                var _smt_lp_model_contract_value_formatted = result["_smt_lp_model_contract_value@OData.Community.Display.V1.FormattedValue"];
                var _smt_lp_model_contract_value_lookuplogicalname = result["_smt_lp_model_contract_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

                var lpmodel = new Array();
                lpmodel[0] = new Object();
                lpmodel[0].id = _smt_lp_model_contract_value;
                lpmodel[0].name = _smt_lp_model_contract_value_formatted;
                lpmodel[0].entityType = _smt_lp_model_contract_value_lookuplogicalname;
                formContext.getAttribute("smt_lp_modelcontract").setValue(lpmodel);
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
                return result;
            }
        );
    },

    getPurchasePeriod: function (resourceId) {
        "use strict";
        // debugger;

        var globalContext = Xrm.Utility.getGlobalContext();
        var result = null;

        //var req = new XMLHttpRequest();
        //req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/bookableresources(" + resourceId + ")?$expand=smt_lp_model_contract($select=smt_dc_purchansing_period)", false);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 200) {
        //            result = JSON.parse(this.response);
        //            var smt_lp_model_contract_smt_dc_purchansing_period = result["smt_lp_model_contract"]["smt_dc_purchansing_period"];
        //        } else {
        //            PurchasingPeriod.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();

        Xrm.WebApi.online.retrieveRecord("bookableresource", resourceId, "?$expand=smt_lp_model_contract($select=smt_dc_purchansing_period)").then(
            function success(result) {
                var bookableresourceid = result["bookableresourceid"];
                for (var a = 0; a < result.smt_bookableresource_smt_purchasing_period_lp_resource.length; a++) {
                    var smt_lp_model_contract_smt_dc_purchansing_period = result["smt_lp_model_contract"]["smt_dc_purchansing_period"];
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
        return result;
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
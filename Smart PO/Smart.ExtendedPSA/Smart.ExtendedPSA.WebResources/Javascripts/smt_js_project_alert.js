if (typeof (ProjectAlert) === undefined || typeof (ProjectAlert) === "undefined") { ProjectAlert = {}; }

var ProjectAlert_LogicalName = "msdyn_project";

ProjectAlert.Functions = {

    AlertConclusionForm: function (primaryControl) {
        "use strict";
        formContext = primaryControl;

        var id = formContext.data.entity.getId();
        id = id.replace("{", "").replace("}", "");

        var globalContext = Xrm.Utility.getGlobalContext();
        var confirmStrings = { text: "", title: "Concluir Alerta de Projeto", subtitle: "Deseja realmente concluir este Alerta de Projeto?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                // debugger;
                if (success.confirmed) {
                    var execute_smt_ACInactivateProjectAlert_Request = {
                        // Parameters
                        entity: { entityType: "smt_project_alerts", id: id }, // entity

                        getMetadata: function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    entity: { typeName: "mscrm.smt_project_alerts", structuralProperty: 5 }
                                },
                                operationType: 0, operationName: "smt_ACInactivateProjectAlert"
                            };
                        }
                    };

                    await Xrm.WebApi.execute(execute_smt_ACInactivateProjectAlert_Request).then(
                        function success(response) {
                            if (response.ok) {
                                formContext.data.refresh();
                            }
                        }
                    ).catch(function (error) {
                        ProjectAlert.Functions.showAlertDialog(this.statusText);
                    });
                    /*
                    var req = new XMLHttpRequest();
                    req.open("POST", globalContext.getClientUrl() + "/api/data/v9.1/smt_project_alertses(" + id + ")/Microsoft.Dynamics.CRM.smt_ACInactivateProjectAlert", true);
                    req.setRequestHeader("OData-MaxVersion", "4.0");
                    req.setRequestHeader("OData-Version", "4.0");
                    req.setRequestHeader("Accept", "application/json");
                    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                    req.onreadystatechange = function () {
                        if (this.readyState === 4) {
                            req.onreadystatechange = null;
                            if (this.status === 204) {
                                formContext.data.refresh();
                            } else {
                                ProjectAlert.Functions.showAlertDialog(this.statusText);
                            }
                        }
                    };
                    req.send();
                    */
                }
            });
    },

    showAlertDialog: function (message) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: message };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {
                //console.log("Alert dialog closed");
            },
            function (error) {
                //console.log(error.message);
            })
    }
}

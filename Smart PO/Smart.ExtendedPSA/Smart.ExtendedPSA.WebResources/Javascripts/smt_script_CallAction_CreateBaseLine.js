if (typeof (BtnBaseline) === undefined || typeof (BtnBaseline) === "undefined") { BtnBaseline = {}; }

var BtnBaseline_LogicalName = "msdyn_project";
BtnBaseline.Functions = {

    AC_CreateBaseLine: function (primaryControl) {
        "use strict";
        //var formContext = executionContext.getFormContext();
        var idProject = primaryControl.data.entity.getId();
        var parameters = {};
        var entity = {};
        entity.id = (idProject.replace("}", "")).replace("{", "");;
        entity.entityType = "msdyn_project";
        parameters.entity = entity;

        Xrm.Utility.showProgressIndicator("Criando a baseline!");

        var smt_AC_Create_BaseLineRequest = {
            entity: parameters.entity,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.msdyn_project",
                            "structuralProperty": 5
                        }
                    },
                    operationType: 0,
                    operationName: "smt_AC_Create_BaseLine"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_Create_BaseLineRequest).then(
            function success(result) {
                if (result.ok) {
                    Xrm.Utility.closeProgressIndicator();
                    var message = { confirmButtonLabel: "OK", text: "Baseline criada com sucesso." };
                    var alertOptions = { height: 150, width: 280 };
                    Xrm.Navigation.openAlertDialog(message, alertOptions).then(

                    )
                }
            },
            function (error) {
                Xrm.Utility.closeProgressIndicator();
                var message = { confirmButtonLabel: "OK", text: "Erro ao criar a baseline.\n\nErro: " + error.message };
                var alertOptions = { height: 150, width: 280 };
                Xrm.Navigation.openAlertDialog(message, alertOptions);
                //showAlertDialog(error.message);
            }
        );
    },

    showAlertDialog: function (message_) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: message_ };
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

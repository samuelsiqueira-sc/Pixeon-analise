if (typeof (CancelProjectTask) === undefined || typeof (CancelProjectTask) === "undefined") { CancelProjectTask = {}; }

var CancelProjectTask_LogicalName = "msdyn_projecttask";
CancelProjectTask.Functions = {

    inativaTarefaProjeto: function (primaryControl) {
        "use strict";
        var formContext = primaryControl;

        var confirmStrings = {
            text: "", title: "Confirmação de cancelamento.", subtitle: "Deseja realmente cancelar a tarefa selecionada?",
            "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM"
        };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var parameters = {};
                    var entity = {};
                    var entityname_ = formContext.data.entity.getEntityName();
                    var id = formContext.data.entity.getId();
                    id = id.replace("{", "").replace("}", "");
                    entity.id = id;
                    entity.entityType = entityname_;
                    parameters.entity = entity;

                    var smt_AC_Deactivate_ProjectTaskRequest = {
                        entity: parameters.entity,

                        getMetadata: function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    "entity": {
                                        "typeName": "mscrm.msdyn_projecttask",
                                        "structuralProperty": 5
                                    }
                                },
                                operationType: 0,
                                operationName: "smt_AC_Deactivate_ProjectTask"
                            };
                        }
                    };

                    Xrm.WebApi.online.execute(smt_AC_Deactivate_ProjectTaskRequest).then(
                        function success(result) {
                            if (result.ok) {

                                formContext.data.refresh();
                            }
                        },
                        function (error) {
                            CancelProjectTask.Functions.showAlertDialog(error.message);
                        }
                    );

                }

            });
    },


    reactivateProjectTask: function (primaryControl) {
        "use strict";
        var formContext = primaryControl;

        var confirmStrings = {
            text: "", title: "Confirmação de reativação", subtitle: "Deseja realmente reativar a tarefa do projeto selecionada?",
            "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM"
        };
        var confirmOptions = { height: 200, width: 500 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var parameters = {};
                    var entity = {};
                    var entityname_ = formContext.data.entity.getEntityName();
                    var id = formContext.data.entity.getId();
                    id = id.replace("{", "").replace("}", "");
                    entity.id = id;
                    entity.entityType = entityname_;
                    parameters.entity = entity;

                    var smt_AC_Reativar_Tarefa_do_ProjetoRequest = {
                        entity: parameters.entity,

                        getMetadata: function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    "entity": {
                                        "typeName": "mscrm.msdyn_projecttask",
                                        "structuralProperty": 5
                                    }
                                },
                                operationType: 0,
                                operationName: "smt_AC_Reativar_Tarefa_do_Projeto"
                            };
                        }
                    };

                    Xrm.WebApi.online.execute(smt_AC_Reativar_Tarefa_do_ProjetoRequest).then(
                        function success(result) {
                            if (result.ok) {
                                formContext.data.refresh();
                            }
                        },
                        function (error) {
                            CancelProjectTask.Functions.showAlertDialog(error.message);
                        }
                    );
                }

            }
        );
    },

    showAlertDialog: function (message_) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: message_ };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {
                //  console.log("Alert dialog closed");
            },
            function (error) {
                //   console.log(error.message);
            })
    }

}
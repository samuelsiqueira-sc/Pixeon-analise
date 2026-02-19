if (typeof (HolidayRequest) === undefined || typeof (HolidayRequest) === "undefined") { HolidayRequest = {}; }
/*
Utilizar comando sempre para cada nova função (trigger)
*/
var HolidayRequest_LogicalName = "smt_holiday_request";
HolidayRequest.Functions = {
    /*Evento default registrado no Onload do formulário*/
    OnLoad: function (_executionContext) {
        "use strict";
        //var smartHelper = new SmartHelper(_executionContext);
        this.getBookableResourceRequest(_executionContext);
    },

    CustomFunction: function (smartHelper) {

    },

    /*Função chamada por um botão na ribbon. Recebe o PrimaryControl(formContext)*/
    callActionSendEmail: function (primaryControl) {
        "use strict";
        var confirmStrings = { text: "", title: "Enviar solicitação de férias", subtitle: "Deseja realmente enviar essa solicitação de férias?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    primaryControl.getAttribute("statuscode").setValue(100000000);
                    var guid = primaryControl.data.entity.getId();
                    primaryControl.data.entity.save("smt_holiday_request");
                }
            });
    },

    RecoverHolidayRequest: function (primaryControl) {
        "use strict";
        var confirmStrings = { text: "", title: "Recuperar solicitação de férias", subtitle: "Deseja realmente recuperar esta solicitação de férias?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var guid = primaryControl.data.entity.getId();
                    guid = guid.replace("{", "").replace("}", "");
                    var parameters = {};
                    var entity = {};
                    entity.id = guid;
                    entity.entityType = "smt_holiday_request";
                    parameters.entity = entity;

                    var smt_ACRecoverHolidayRequestRequest = {
                        entity: parameters.entity,

                        getMetadata: function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    "entity": {
                                        "typeName": "mscrm.smt_holiday_request",
                                        "structuralProperty": 5
                                    }
                                },
                                operationType: 0,
                                operationName: "smt_ACRecoverHolidayRequest"
                            };
                        }
                    };

                    Xrm.WebApi.online.execute(smt_ACRecoverHolidayRequestRequest).then(
                        function success(result) {
                            if (result.ok) {
                                //Success - No Return Data - Do Something
                                primaryControl.data.refresh();
                            }
                        },
                        function (error) {
                            Xrm.Utility.alertDialog(error.message);
                        }
                    );
                }
            });
    },

    cancelHolidayRequest: function (primaryControl) {
        "use strict";
        if (primaryControl.data.entity.getEntityName() == HolidayRequest_LogicalName && primaryControl.getAttribute("statuscode").getValue() == 1) {
            var holidayRequestLookup = {
                entityType: HolidayRequest_LogicalName,
                id: primaryControl.data.entity.getId()
            };

            Xrm.Utility.openQuickCreate("smt_cancel_holiday_request", holidayRequestLookup);

        }
    },

    rejectHolidayRequestVisible: function (primaryControl) {
        "use strict";
        var showButton = false;
        // validar se o campo recurso está presente no formulário (getControl() != null ? )
        if (primaryControl.getAttribute("statuscode").getValue() == 100000000) {

            if (primaryControl.getAttribute("smt_lp_resource").getValue() != null) {
                //pegando o id do Recurso
                var idResource = primaryControl.getAttribute("smt_lp_resource").getValue()[0].id;

                //var userId = ExtendedHelp.Functions.retrieveUserId(idResource);
                // function que retorna a UnidadeOrganizacional
                var idUnitOrg = ExtendedHelp.Functions.retrieveUnitOrganization(idResource);

                //validar se o retrieve não veio nulo ( != undefined && != null && > 0)
                if (idUnitOrg != undefined && idUnitOrg != null) {

                    //Variável do nome para a busca de parâmetros de gerente

                    //Realizar a busca em parametros (tornar o mais generico possivel)
                    var parameterManager = ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrg._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.ManagerParameterHoliday);

                    if (parameterManager != undefined && parameterManager != null) { // COLOCAR && parameterManager.value.length > 0
                        //Realizar busca em equipe que tem o mesmo nome do valor de Paramêtro
                        var team = ExtendedHelp.Functions.retrieveTeam(parameterManager);

                        if (team != undefined && team != null) {
                            var userId = Xrm.Utility.getGlobalContext().userSettings.userId;

                            //Realizar busca em TeamMemberShip onde teamid = team && userid = idResource
                            var isTeam = ExtendedHelp.Functions.retrieveTeamMemberShip(userId, team);
                            if (isTeam == true) {
                                showButton = true;
                            }
                        }
                    }
                }
            }
        }

        return showButton;
    },

    ChangeStatusCodeOnApproval: function (primaryControl) {
        "use strict";
        var confirmStrings = { text: "", title: "Aprovar solicitação de férias", subtitle: "Deseja realmente aprovar essa solicitação de férias?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    var guid = primaryControl.data.entity.getId();
                    var statuscode = 2;
                    ExtendedHelp.Functions.updateStatusCodeApprove(guid, statuscode);
                }
            });
    },

    getBookableResourceRequest: function (_executionContext) {
        "use strict";
        var formContext = _executionContext.getFormContext();
        var bookable = { isFault: false, data: [], erro: null };

        Xrm.WebApi.online.retrieveMultipleRecords("bookableresource", "?$select=bookableresourceid,name&$filter=_userid_value eq " + Xrm.Utility.getGlobalContext().userSettings.userId.replace("{", "").replace("}", "")).then(
            function success(response) {
                for (var i = 0; i < response.entities.length; i++) {
                    bookable.data[i] = {
                        bookableresourceid: response.entities[i]["bookableresourceid"],
                        name: response.entities[i]["name"]
                    }
                }
                var formType = formContext.ui.getFormType();
                var recurso = formContext.getAttribute("smt_lp_resource").getValue();

                if (formType == 1) {
                    if (recurso !== null || recurso !== "" || recurso !== undefined) {
                        if (bookable != null && bookable != undefined && bookable.data.length > 0) {
                            var lookup = new Array();
                            lookup[0] = new Object();
                            lookup[0].name = bookable.data[0].name;
                            lookup[0].id = bookable.data[0].bookableresourceid;
                            lookup[0].entityType = "bookableresource";

                            formContext.getAttribute("smt_lp_resource").setValue(lookup);
                        }
                        else {
                            this.showAlertDialog("O seu usuário não está cadastrado como recurso reservável, por favor contate o administrador/gerente");
                        }
                    } else {
                        var message = "Não é possível criar a solicitação de férias pois não há um registro de recurso reservável em seu nome, por favor contatar o seu gerente";
                        formContext.ui.setFormNotification(message, "warning", "naoPreenchida");
                    }
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
    },

    retrieveBookableResource: function() {
        "use strict";
        var results = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/bookableresources?$select=bookableresourceid,name&$filter=_userid_value eq " + Xrm.Utility.getGlobalContext().userSettings.userId.replace("{", "").replace("}", "") + "", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        var bookableresourceid = results.value[i]["bookableresourceid"];
                        var name = results.value[i]["name"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    rejectHolidayRequest: function (primaryControl) {
        "use strict";
        var confirmStrings = { text: "", title: "Reprovar solicitação de férias", subtitle: "Deseja realmente reprovar essa solicitação de férias?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    primaryControl.getControl("smt_st_justification").setDisabled(false);
                    primaryControl.getAttribute("smt_st_justification").setRequiredLevel("required");
                    primaryControl.getControl("smt_st_justification").setVisible(true);
                    if (primaryControl.getAttribute("smt_st_justification").getValue() != null) {
                        var status = 100000002;
                        var guid = primaryControl.data.entity.getId();
                        ExtendedHelp.Functions.updateStatusCode(guid, status);

                    } else {
                        Xrm.Utility.alertDialog("A justificativa precisa ser preenchida para continuar com o processo de reprovação.");
                    }

                }
            }
        );
    },

    OpenQuickCreatAgendaColetiva: function (selectedIds) {
        "use strict";
        // Converte os valores do Id obtidos pelo btn em Strings 
        var strIds = selectedIds.toString();


        var entityFormOptions = {};
        entityFormOptions["entityName"] = "smt_button_dialog";
        entityFormOptions["useQuickCreateForm"] = true;

        //Adiciona o Id no campo RegisterID e o tipo da ação no Name (no caso férias coletivas) 
        var parameters = {};
        //parameters["formid"] = "E8660BB2-1626-49DE-BCAF-88C75C7643D1";
        parameters["smt_tx_register_id"] = strIds;
        parameters["smt_name"] = "Férias Coletivas";
        parameters["smt_pl_confirmation_type"] = 1;

        //Chamada da criação rápida
        Xrm.Navigation.openForm(entityFormOptions, parameters).then(function () { successCallback(); }, function () { errorCallback(); });

    },

    // Caso a chamada da criação rápida funcione
    successCallback: function () {

    },

    // Caso a chamada da criaçãa rápida falhe
    errorCallback: function () {

    },

    regraBtnEnviar: function (primaryControl) {
        "use strict";
        var lpRecurso = primaryControl.getAttribute('smt_lp_resource');
        var idUsuarioRegistro = lpRecurso.getValue()[0].id.replace("{", "").replace("}", ""); //Id do recurso apontado no lookup
        var retrieveRecurso = function (id) {
            var result = null;
            var req = new XMLHttpRequest();
            req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/bookableresources(" + id + ")?$select=_userid_value", false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        result = JSON.parse(this.response);
                        var _userid_value = result["_userid_value"];
                        var _userid_value_formatted = result["_userid_value@OData.Community.Display.V1.FormattedValue"];
                        var _userid_value_lookuplogicalname = result["_userid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);

                    }
                }
            };
            req.send();

            return result;
        };
        var recurso = retrieveRecurso(idUsuarioRegistro);
        var idSystemUser = recurso["_userid_value"].toLowerCase(); // Id do Usuario apontado em recurso
        var configUsuario = Xrm.Utility.getGlobalContext().userSettings;
        var idUsuarioAcessa = configUsuario.userId.replace("{", "").replace("}", "").toLowerCase(); //Id do usuario que acessa o registro
        if (idSystemUser !== null || idSystemUser !== "" || idSystemUser !== undefined) {
            if (idUsuarioAcessa !== null || idUsuarioAcessa !== "" || idUsuarioAcessa !== undefined) {
                if (idSystemUser == idUsuarioAcessa) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
    },


    GetSecuretRoleManager: async function () {
        "use strict";
        var userSettings = Xrm.Utility.getGlobalContext().userSettings.userId;
        var GetData = await ExtendedHelp.Functions.request(userSettings);
        var foundSecurityRole = false;
        var total = GetData.value.length;
        for (var i = 0; i < total; i++) {
            if (GetData.value[i].name == "Extended Project Manager") {
                return true;
            }
            if (GetData.value[i].name == "Project Manager") {
                return true;
            }
        }

        return foundSecurityRole;

    },

    rejectHoliday: function (primaryControl) {
        "use strict";
        var showButton = false;

        if (primaryControl.getAttribute("statuscode").getValue() == 2) {
            // validar se o campo recurso está presente no formulário (getControl() != null ? )
            if (primaryControl.getAttribute("smt_lp_resource").getValue() != null) {
                //pegando o id do Recurso
                var idResource = primaryControl.getAttribute("smt_lp_resource").getValue()[0].id;
                var userId = Xrm.Utility.getGlobalContext().userSettings.userId;

                //var userId = ExtendedHelp.Functions.retrieveUserId(idResource);
                // function que retorna a UnidadeOrganizacional
                var idUnitOrg = ExtendedHelp.Functions.retrieveUnitOrganization(idResource);

                //validar se o retrieve não veio nulo ( != undefined && != null && > 0)
                if (idUnitOrg != undefined && idUnitOrg != null) {

                    //Variável do nome para a busca de parâmetros de gerente

                    //Realizar a busca em parametros (tornar o mais generico possivel)
                    var parameterManager = ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrg._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.ManagerParameterHoliday);

                    if (parameterManager != undefined && parameterManager != null) { // COLOCAR && parameterManager.value.length > 0
                        //Realizar busca em equipe que tem o mesmo nome do valor de Paramêtro
                        var team = ExtendedHelp.Functions.retrieveTeam(parameterManager);

                        if (team != undefined && team != null) {

                            //Realizar busca em TeamMemberShip onde teamid = team && userid = idResource
                            var isTeam = ExtendedHelp.Functions.retrieveTeamMemberShip(userId, team);
                            if (isTeam == true) {
                                showButton = true;
                            }
                        }
                    }

                    //Realizar a busca em parametros (tornar o mais generico possivel)
                    var parameterRH = ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrg._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.RHParameterHoliday);

                    if (parameterRH != undefined && parameterRH != null) { // COLOCAR && parameterManager.value.length > 0
                        //Realizar busca em equipe que tem o mesmo nome do valor de Paramêtro
                        var teamRH = ExtendedHelp.Functions.retrieveTeam(parameterRH);

                        if (teamRH != undefined && teamRH != null) {

                            //Realizar busca em TeamMemberShip onde teamid = team && userid = idResource
                            var isTeamRH = ExtendedHelp.Functions.retrieveTeamMemberShip(userId, teamRH);
                            if (isTeamRH == true) {
                                showButton = true;
                            }
                        }
                    }

                }
            }
        }

        return showButton;
    },

    OpenQuickCreateCancel: function (id, primaryControl) {
        "use strict";
        // Converte os valores do Id obtidos pelo btn em Strings 
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "smt_button_dialog";
        entityFormOptions["useQuickCreateForm"] = true;

        //Adiciona o Id no campo RegisterID e o tipo da ação no Name (no caso férias coletivas) 
        var parameters = {};
        parameters["smt_tx_register_id"] = id[0];
        parameters["smt_name"] = "Cancelamento de Férias";
        parameters["smt_pl_confirmation_type"] = 2;

        //Chamada da criação rápida
        Xrm.Navigation.openForm(entityFormOptions, parameters).then(function () { successCallback(); }, function () { errorCallback(); });

    },

    createUpdateHolidayRequest: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        if (formContext.getAttribute("smt_pl_confirmation_type").getValue() == 1) {
            //var ids = formContext.getAttribute("smt_tx_register_id").getValue();
            //var arrIds = ids.split(",");
            //var start = formContext.getAttribute("smt_dt_holiday_start_date").getValue();
            //var end = formContext.getAttribute("smt_dt_holiday_end_date").getValue();

            //for (var indxIds = 0; indxIds < arrIds.length; indxIds++) {
            //    this.createHolidayRequest(arrIds[indxIds], start, end);
            //}
        }
        else {
            var justificativa = formContext.getAttribute("smt_st_justification").getValue();
            var id = formContext.getAttribute("smt_tx_register_id").getValue();
            this.updateHolidayRequest(id, justificativa);
        }
    },

    createHolidayRequest: function (id, start, end) {
        "use strict";
        var entity = {};
        entity.smt_dt_end = end;
        entity.smt_dt_start = start;
        entity["smt_lp_resource@odata.bind"] = "/bookableresources(" + id + ")";
        entity.smt_smt_pl_type = 100000001;

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_holiday_requests", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    var uri = this.getResponseHeader("OData-EntityId");
                    var regExp = /\(([^)]+)\)/;
                    var matches = regExp.exec(uri);
                    var newEntityId = matches[1];
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(entity));
    },

    updateHolidayRequest: function (id, justificativa) {
        "use strict";
        var parameters = {};
        var entity = {};
        entity.id = id.replace("{", "").replace("}", "");
        entity.entityType = "smt_holiday_request";
        parameters.entity = entity;
        parameters.motivo = justificativa;

        var smt_ACCancelHolidayRequestRequest = {
            entity: parameters.entity,
            motivo: parameters.motivo,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.smt_holiday_request",
                            "structuralProperty": 5
                        },
                        "motivo": {
                            "typeName": "Edm.String",
                            "structuralProperty": 1
                        }
                    },
                    operationType: 0,
                    operationName: "smt_ACCancelHolidayRequest"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_ACCancelHolidayRequestRequest).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
    },


    HideShow_smt_pl_type: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        var OptionSetValue;

        OptionSetValue = formContext.getAttribute("smt_smt_pl_type").getValue();


        if (OptionSetValue != 100000001) // Verifica se foi trazido algum reesultado no request
        {
            formContext.getControl("smt_smt_pl_type").removeOption(100000001);
        }
        else if (OptionSetValue == 100000001) {
            formContext.getControl("smt_smt_pl_type").setDisabled(true);
        }

    },

    ReactivateVacationRequest: function (primaryControl) {
        "use strict";
        var confirmStrings = { text: "", title: "Reativar solicitação de férias", subtitle: "Deseja realmente reativar essa solicitação de férias?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var guid = primaryControl.data.entity.getId();
                    guid = guid.replace("{", "").replace("}", "");
                    var parameters = {};
                    var entity = {};
                    entity.id = guid;
                    entity.entityType = "smt_holiday_request";
                    parameters.entity = entity;

                    var smt_AC_ReactivateVacationRequestRequest = {
                        entity: parameters.entity,

                        getMetadata: function () {
                            return {
                                boundParameter: "entity",
                                parameterTypes: {
                                    "entity": {
                                        "typeName": "mscrm.smt_holiday_request",
                                        "structuralProperty": 5
                                    }
                                },
                                operationType: 0,
                                operationName: "smt_AC_ReactivateVacationRequest"
                            };
                        }
                    };

                    Xrm.WebApi.online.execute(smt_AC_ReactivateVacationRequestRequest).then(
                        function success(result) {
                            if (result.ok) {
                                primaryControl.data.refresh();
                            }
                        },
                        function (error) {
                            Xrm.Utility.alertDialog(error.message);
                        }
                    );
                }


            });



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
};

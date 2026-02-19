if (typeof (ResourceDeparture) === undefined || typeof (ResourceDeparture) === "undefined") { ResourceDeparture === {}; }
ResourceDeparture.Functions = {
    statusSubmit: function (formContext) {
        "use strict";
        // Altera o Status do Registro de 'Rascunho' para 'Enviado'
        if (formContext.getAttribute("statuscode").getValue() === 1) {
            formContext.getAttribute("statuscode").setValue(100000000); //Status igual a Enviado
            // var guid = formContext.data.entity.getId();
            formContext.data.entity.save("smt_resource_departure");
        }
    },

    visibleButtonSubmit: async function (formContext) {
        "use strict";
        // debugger;
        var lpRecurso = formContext.getAttribute('smt_lp_resource');
        var idResourceUser = lpRecurso.getValue()[0].id;
        var recurso = await ExtendedHelp.Functions.retrieveUserId(idResourceUser);

        var idSystemUser = recurso["_userid_value"].toLowerCase(); // Id do Usuario apontado em recurso
        var configUsuario = Xrm.Utility.getGlobalContext().userSettings;
        var idUsuarioAcessa = configUsuario.userId.replace("{", "").replace("}", "").toLowerCase(); //Id do usuario que acessa o registro

        if (idSystemUser === idUsuarioAcessa) {
            return true;
        }

        if (idUsuarioAcessa !== null || idUsuarioAcessa !== "" || idUsuarioAcessa !== undefined) {
            var idUnitOrganization = await ExtendedHelp.Functions.retrieveUnitOrganization(idResourceUser);
            var parameterManager = await ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrganization._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.ManagerParameterHoliday);
            if (idSystemUser !== null || idSystemUser !== "" || idSystemUser !== undefined) {
                if (idUnitOrganization !== undefined && idUnitOrganization !== null) {
                    if (parameterManager !== undefined && parameterManager !== null) {
                        var team = await ExtendedHelp.Functions.retrieveTeam(parameterManager);
                        if (team !== undefined && team !== null) {
                            var isTeam = await ExtendedHelp.Functions.retrieveTeamMemberShip(idUsuarioAcessa, team);
                            if (isTeam === true) { return true; }
                        }
                    }
                }
                if (idSystemUser === idUsuarioAcessa) {
                    return true;
                }
            }
        } else {
            return false;
        }
    },

    visibleButtonCancel: async function (formContext) {
        "use strict";
        var showButton = false;
        // Status igual a aprovado
        if (formContext.getAttribute("statuscode").getValue() === 100000002) {
            if (formContext.getAttribute("smt_lp_resource").getValue() !== null) {
                var idResource = formContext.getAttribute("smt_lp_resource").getValue()[0].id;
                var idUnitOrganization = await ExtendedHelp.Functions.retrieveUnitOrganization(idResource);

                if (idUnitOrganization !== undefined && idUnitOrganization !== null) {
                    var parameterManager = await ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrganization._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.ManagerParameterHoliday);
                    if (parameterManager !== undefined && parameterManager !== null) {
                        var team = await ExtendedHelp.Functions.retrieveTeam(parameterManager);
                        if (team !== undefined && team !== null) {
                            var userId = Xrm.Utility.getGlobalContext().userSettings.userId;
                            var isTeam = await ExtendedHelp.Functions.retrieveTeamMemberShip(userId, team);
                            if (isTeam === true) {
                                showButton = true;
                            }
                        }
                    }
                }
            }
        }
        return showButton;
    },

    visibleButtonApprovedAndDisapproved: async function (formContext) {
        "use strict";
        var showButton = false;
        // Status igual a enviado
        if (formContext.getAttribute("statuscode").getValue() === 100000000) {
            if (formContext.getAttribute("smt_lp_resource").getValue() !== null) {
                var idResource = formContext.getAttribute("smt_lp_resource").getValue()[0].id;
                var idUnitOrganization = await ExtendedHelp.Functions.retrieveUnitOrganization(idResource);
                if (idUnitOrganization !== undefined && idUnitOrganization !== null) {
                    var parameterManager = await ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrganization._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.ManagerParameterHoliday);
                    if (parameterManager !== undefined && parameterManager !== null) {
                        var team = await ExtendedHelp.Functions.retrieveTeam(parameterManager);
                        if (team !== undefined && team !== null) {
                            var userId = Xrm.Utility.getGlobalContext().userSettings.userId;
                            var isTeam = await ExtendedHelp.Functions.retrieveTeamMemberShip(userId, team);
                            if (isTeam === true) {
                                showButton = true;
                            }
                        }
                    }

                    var parameterRH = await ExtendedHelp.Functions.retrieveParameterSmart(idUnitOrganization._msdyn_organizationalunit_value, ExtendedHelp.Functions.ParametersName.RHParameterHoliday);
                    if (parameterRH !== undefined && parameterRH !== null) {
                        var teamRH = await ExtendedHelp.Functions.retrieveTeam(parameterRH);
                        if (teamRH !== undefined && teamRH !== null) {
                            var isTeamRH = await ExtendedHelp.Functions.retrieveTeamMemberShip(userId, teamRH);
                            if (isTeamRH === true) {
                                showButton = true;
                            }
                        }
                    }
                }
            }
        }
        return showButton;
    },

    statusDraft: function (formContext) {
        "use strict";
        var alertStrings = { text: "", title: "Cancelar Afastamento", subtitle: "Deseja realmente cancelar esse afastamento?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var alertOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(alertStrings, alertOptions).then(
            function (sucess) {
                if (sucess.confirmed) {
                    if (formContext.getAttribute("statuscode").getValue() === 100000002 ||
                        formContext.getAttribute("statuscode").getValue() === 100000000 ||
                        formContext.getAttribute("statuscode").getValue() === 2) {

                        formContext.getAttribute("statecode").setValue(1);
                        formContext.getAttribute("statuscode").setValue(100000003);
                        formContext.data.entity.save("smt_resource_departure");
                    }
                }
            })
    },

    statusApprovedAndSendEmail: function (formContext) {
        "use strict";
        var alertStrings = { text: "", title: "Aprovar Afastamento", subtitle: "Deseja realmente aprovar esse afastamento?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var alertOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(alertStrings, alertOptions).then(
            function (sucess) {
                if (sucess.confirmed) {
                    if (formContext.getAttribute("statuscode").getValue() === 100000000) {
                        formContext.getAttribute("statecode").setValue(1);
                        formContext.getAttribute("statuscode").setValue(100000002);
                        formContext.data.entity.save("smt_resource_departure");
                    }
                }
            })
    },

    statusDisapprovedAndSendEmail: function (formContext) {
        "use strict";
        var alertStrings = { text: "", title: "Reprovar Afastamento", subtitle: "Deseja realmente reprovar esse afastamento?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
        var alertOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(alertStrings, alertOptions).then(
            function (sucess) {
                if (sucess.confirmed) {
                    if (formContext.getAttribute("statuscode").getValue() === 100000000) {
                        formContext.getAttribute("statecode").setValue(1);
                        formContext.getAttribute("statuscode").setValue(2);
                        formContext.data.entity.save("smt_resource_departure");
                    }
                }
            })
    },
}

if (typeof (Project) === undefined || typeof (Project) === "undefined") { Project = {}; }

var Project_LogicalName = "account";
Project.Functions = {
    createDocumento: function (executionContext, projectId, orgId) {
        debugger;
        var url = null;

        if (orgId == "orgad9fb8c0") {
            url = "https://prod-07.brazilsouth.logic.azure.com:443/workflows/3f0a406605e34957a07b895f391e5ec5/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=oj8GhRXshGzPsGArzv8k9CKjq5a8380dNQWP-lEaWS8";
        }
        else {
            url = "https://prod-14.brazilsouth.logic.azure.com:443/workflows/8e567e3395ac46069a3d6eefac18684c/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=wnI3rjM1zeZPxa63hGrktoNwBbaVGXISYicLCXyVWH4";
        }

        if (url != null) {
            var settings = {
                "async": true,
                "crossDomain": true,
                "url": url,
                "method": "POST",
                "headers": {
                    "Content-Type": "application/json",
                    "User-Agent": "PostmanRuntime/7.19.0",
                    "Accept": "*/*",
                    "Cache-Control": "no-cache",
                    "Postman-Token": "2d02d71b-358f-49cf-94bb-c62d9f6adcb7,f254029b-e0d7-405f-8fad-209a2779c59a",
                    "Host": "prod-15.brazilsouth.logic.azure.com:443",
                    "Accept-Encoding": "gzip, deflate",
                    "Content-Length": "0",
                    "Connection": "keep-alive",
                    "cache-control": "no-cache"
                },
                "processData": false,
                "data": "{\r\n \"project\": \"" + projectId + "\",\n}"
            }
            $.ajax(settings).done(function (data) {
                $('#status').empty();
                $('#status').append("");
                parent.location.reload();
            }).fail(function (jqXHR, textStatus) {
                $('#status').empty();
                $('#status').append(jqXHR.responseText);
                var alertStrings = { confirmButtonLabel: "OK", text: jqXHR.responseText };
                var alertOptions = { height: 120, width: 260 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function success(result) {
                        //console.log("Alert dialog closed");
                    },
                    function (error) {
                        //console.log(error.message);
                    }
                );
            });
        }
    },

    verificaGerenteTemporario: function (executionContext) {
        debugger;
        var formContext = executionContext.getFormContext();

        var gerenteTemporario = formContext.getAttribute("smt_lp_temporary_manager").getValue();
        var id = Xrm.Page.data.entity.getId();

        if (gerenteTemporario != null) {
            var resource = Project.Functions.getResource(formContext.getAttribute("smt_lp_temporary_manager").getValue()[0].id.replace("{", "").replace("}", ""));

            if (resource != undefined && resource != null && resource.value.length > 0) {
                var projectTeam = Project.Functions.getResourceTeam(resource.value[0].bookableresourceid, id.replace("{", "").replace("}", ""));

                if (projectTeam != undefined && projectTeam != null && projectTeam.value.length > 0) {
                    if (projectTeam.value[0].msdyn_projectapprover == false) {
                        Xrm.Page.ui.setFormNotification("O gerente temporário não está definido como aprovador do projeto, isso pode impactar no processo de aprovação das horas. Por favor, verificar a informação.", "WARNING");

                    }
                }
            }
        }
    },

    getResource: function (id) {
        var results = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/bookableresources?$select=bookableresourceid,_userid_value&$filter=_userid_value eq " + id, false);
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
                        var _userid_value = results.value[i]["_userid_value"];
                        var _userid_value_formatted = results.value[i]["_userid_value@OData.Community.Display.V1.FormattedValue"];
                        var _userid_value_lookuplogicalname = results.value[i]["_userid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    getResourceTeam: function (id, idProjeto) {
        var results = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/msdyn_projectteams?$select=msdyn_projectapprover&$filter=_msdyn_bookableresourceid_value eq " + id + " and _msdyn_project_value eq " + idProjeto, false);
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
                        var msdyn_projectapprover = results.value[i]["msdyn_projectapprover"];
                        var msdyn_projectapprover_formatted = results.value[i]["msdyn_projectapprover@OData.Community.Display.V1.FormattedValue"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    getAlerts: function (executionContext) {
        var formContext = executionContext.getFormContext();

        var alerts = Project.Functions.searchAlerts(Xrm.Page.data.entity.getId());

        if (alerts != undefined && alerts != null && alerts.value.length > 0) {
            Xrm.Page.ui.setFormNotification("Há inconsistencias no projeto. Para verificar essas informações, por favor acessar a guia de 'Alertas do Projeto'", "WARNING", "notificationAlert");
        }
    },

    searchAlerts: function (id) {
        var results = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_project_alertses?$filter=_smt_lp_project_value eq " + id.replace("{", "").replace("}", "") + " and  statuscode ne 2", false);
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
                        var smt_project_alertsid = results.value[i]["smt_project_alertsid"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    visibleButton: function (id) {
        debugger;
        if (Xrm.Page.ui.getFormType() == 2) {
            id = id.replace("{", "").replace("}", "");
            var alerts = Project.Functions.getAlertsForm(id);

            if (alerts != undefined && alerts != null && alerts.value.length > 0) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    },

    AlertsForms: function (id, executionContext) {
        debugger;

        var confirmStrings = { text: "Essa ação irá concluir todos os alertas que já foram corrigidas, deseja mesmo continuar?", title: "Confirmação de Conclusão de Alertas", "cancelButtonLabel": "Cancelar", "confirmButtonLabel": "Sim" };
        var confirmOptions = { height: 200, width: 500 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    Xrm.Utility.showProgressIndicator('Aguarde, estamos concluindo os alertas...');

                    id = id.replace("{", "").replace("}", "");

                    var alerts = Project.Functions.getAlertsForm(id);

                    if (alerts != undefined && alerts != null && alerts.value.length > 0) {
                        for (var a = 0; a < alerts.value.length; a++) {

                            var task = Project.Functions.getTask(results.value[a]._smt_lp_taskproject_value);
                            if (task != null && task.msdyn_scheduledend != null && task.smt__dt_estimated != null) {
                                var scheduleEnd = new Date(task.msdyn_scheduledend).format("dd/MM/yyyy");
                                var smt__dt_estimated = new Date(task.smt__dt_estimated).format("dd/MM/yyyy");

                                if (scheduleEnd == smt__dt_estimated) {
                                    var req = new XMLHttpRequest();
                                    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_project_alertses(" + alerts.value[a].smt_project_alertsid + ")/Microsoft.Dynamics.CRM.smt_ActionFechaAlertadoProjeto", false);
                                    req.setRequestHeader("OData-MaxVersion", "4.0");
                                    req.setRequestHeader("OData-Version", "4.0");
                                    req.setRequestHeader("Accept", "application/json");
                                    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                                    req.onreadystatechange = function () {
                                        if (this.readyState === 4) {
                                            req.onreadystatechange = null;
                                            if (this.status === 204) {
                                                //Success - No Return Data - Do Something
                                            } else {
                                                Xrm.Utility.alertDialog(this.statusText);
                                            }
                                        }
                                    };
                                    req.send();
                                }
                            }
                        }
                        var alerts2 = Project.Functions.getAlertsForm(id);
                        if (alerts2 != undefined && alerts2 != null && alerts2.value.length == 0) {
                            Xrm.Page.ui.clearFormNotification("notificationAlert");
                        }
                        Xrm.Utility.closeProgressIndicator();

                        var alertStrings = { confirmButtonLabel: "OK", text: "Alertas concluídas com sucesso. Caso identifique algum alerta não concluída, por favor verificar se as datas de conclusão e estimada estão corretas." };
                        var alertOptions = { height: 120, width: 260 };
                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function success(result) {
                            },
                            function (error) {
                            }
                        );
                    }
                    else {
                        var alertStrings2 = { confirmButtonLabel: "OK", text: "Esse projeto não possui alertas para serem concluídas." };
                        var alertOptions2 = { height: 120, width: 260 };
                        Xrm.Navigation.openAlertDialog(alertStrings2, alertOptions2).then(
                            function success(result) {

                            },
                            function (error) {

                            }
                        );
                    }
                } else {
                    console.log("Dialog closed using Cancel button or X.");
                }
            });
    },

    getAlertsForm: function (idProject) {
        debugger;
        var result = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_project_alertses?$select=_smt_lp_project_value,_smt_lp_taskproject_value,smt_project_alertsid&$filter=statecode eq 0 and _smt_lp_project_value eq " + idProject, false);
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
                        var _smt_lp_project_value = results.value[i]["_smt_lp_project_value"];
                        var _smt_lp_project_value_formatted = results.value[i]["_smt_lp_project_value@OData.Community.Display.V1.FormattedValue"];
                        var _smt_lp_project_value_lookuplogicalname = results.value[i]["_smt_lp_project_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                        var _smt_lp_taskproject_value = results.value[i]["_smt_lp_taskproject_value"];
                        var _smt_lp_taskproject_value_formatted = results.value[i]["_smt_lp_taskproject_value@OData.Community.Display.V1.FormattedValue"];
                        var _smt_lp_taskproject_value_lookuplogicalname = results.value[i]["_smt_lp_taskproject_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                        var smt_project_alertsid = results.value[i]["smt_project_alertsid"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    getTask: function (idTaks) {
        debugger;
        var result = null;

        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/msdyn_projecttasks(" + idTaks + ")?$select=msdyn_scheduledend,smt__dt_estimated", false);
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
                    var msdyn_scheduledend = result["msdyn_scheduledend"];
                    var smt__dt_estimated = result["smt__dt_estimated"];
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return result;
    },

    onLoad: function (executionContext) {
        debugger;
        var formContext = executionContext.getFormContext();

        var date = new Date();
        var formtDate = date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear();
        var format = new Date(formtDate);

        formContext.getAttribute("msdyn_scheduledstart").setValue(format);
        formContext.getAttribute("msdyn_actualstart").setValue(format);
    },

    openWebResource: function (selectedIds) {
        debugger;
        parameters = new Object();
        parameters.entityId = selectedIds;
        parameters.url = Xrm.Page.context.getClientUrl();

        var customParameters = encodeURIComponent(
            JSON.stringify(parameters)
        );

        var pageInput = {
            pageType: "webresource",
            webresourceName: "smt_html_alterareserva",
            data: customParameters
        };
        var navigationOptions = {
            target: 2,
            width: 500, // value specified in pixel
            height: 400, // value specified in pixel
            position: 1
        };
        Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
            function success(result) {
                alert("teste" + result);
            },
            function error() {
                // Handle errors
            }
        );
    }

};
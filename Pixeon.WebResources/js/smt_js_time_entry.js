// Nome da entidade: Entrada de Hora
if (typeof (TimeEntry) === undefined || typeof (TimeEntry) === "undefined") { TimeEntry = {}; }

var TimeEntry_LogicalName = "smt_purchasing_period";
TimeEntry.Functions = {
    fillFieldsBasedTask: function (executionContext) {
        var formContext = executionContext.getFormContext();

        if (formContext.getAttribute("msdyn_projecttask").getValue() !== null) {

            var idTask = formContext.getAttribute("msdyn_projecttask").getValue()[0].id;

            var retrieve = TimeEntry.Functions.retrieveFieldsProjectTask(idTask.replace("{", "").replace("}", ""));

            var horasRestantes = 0;
            if (retrieve.msdyn_remaininghours !== null) {
                horasRestantes = retrieve.msdyn_remaininghours;

            }
            var percentual = 0;
            if (retrieve.smt_progressofisico !== null) {
                percentual = retrieve.smt_progressofisico;

            }

            formContext.getAttribute("smt_db_hours_left").setValue(horasRestantes);
            formContext.getAttribute("smt_dc_task_percentage").setValue(percentual);
        }
        else {
            formContext.getAttribute("smt_db_hours_left").setValue(null);
            formContext.getAttribute("smt_dc_task_percentage").setValue(null);
        }
    },

    retrieveFieldsProjectTask: function (_id) {
        var globalContext = Xrm.Utility.getGlobalContext();
        var result = null;
        //Retrieve que busca os campos "smt_progressofisico" e "smt_remaininghours" da Entidade "msdyn_projecttask"
        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/msdyn_projecttasks(" + _id.replace("{", "").replace("}", "") + ")?$select=msdyn_remaininghours,smt_progressofisico", false);
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
                    var msdyn_remaininghours = result["msdyn_remaininghours"];
                    var msdyn_remaininghours_formatted = result["msdyn_remaininghours@OData.Community.Display.V1.FormattedValue"];
                    var smt_progressofisico = result["smt_progressofisico"];
                    var smt_progressofisico_formatted = result["smt_progressofisico@OData.Community.Display.V1.FormattedValue"];
                } else {
                    TimeEntry.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();

        return result;
    },

    setFilter: function (executionContext) {
        var formContext = executionContext.getFormContext();
        formContext.getControl("msdyn_projecttask").addPreSearch(TimeEntry.Functions.filterProjectTask);
    },

    filterProjectTask: function (executionContext) {
        var formContext = executionContext.getFormContext();
        var project = formContext.getAttribute("msdyn_project").getValue();
        var projecttask = formContext.getAttribute("msdyn_projecttask").getValue();
        var actualDate = new Date().toISOString().slice(0, 10).replace("-", "/").replace("-", "/");
        var timeEntryFilter = "<filter type='and'>" +
            "<condition value='1' attribute='statuscode' operator='eq' />" +
            "<condition value='100000002' attribute='smt_statusprojeto' operator='ne' />";

        timeEntryFilter = TimeEntry.Functions.getTaskForExclusion(project, timeEntryFilter) + "</filter>";
        formContext.getControl("msdyn_projecttask").addCustomFilter(timeEntryFilter);
        formContext.getControl("msdyn_projecttask").setDefaultView("{4F12E5A8-C14C-4EAB-8A72-AA6C198790F2}");  
    },

    getTaskForExclusion: function (project, timeEntryFilter) {
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/msdyn_projecttasks?$select=_msdyn_parenttask_value&$filter=_msdyn_project_value eq " + project[0].id.replace('{', '').replace('}', ''), false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    var alreadyFiltered = [];
                    for (var i = 0; i < results.value.length; i++) {
                        var _msdyn_parenttask_value = results.value[i]["_msdyn_parenttask_value"];
                        var _msdyn_parenttask_value_formatted = results.value[i]["_msdyn_parenttask_value@OData.Community.Display.V1.FormattedValue"];
                        var _msdyn_parenttask_value_lookuplogicalname = results.value[i]["_msdyn_parenttask_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

                        if (_msdyn_parenttask_value != null) {
                            var taskNotFilterdeYet = true;

                            for (let task of alreadyFiltered) {
                                if (task == _msdyn_parenttask_value_formatted) {
                                    taskNotFilterdeYet = false;
                                    break;
                                }
                            }

                            if (taskNotFilterdeYet) {
                                alreadyFiltered.push(_msdyn_parenttask_value_formatted);
                                timeEntryFilter += "<condition attribute='msdyn_projecttaskid' operator='ne' value='{" + _msdyn_parenttask_value + "}' />";

                            }
                        }
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return timeEntryFilter;
    },

    setFilterHourType: function (executionContext) {
        var formContext = executionContext.getFormContext();
        formContext.getControl("smt_lp_type_hours").addPreSearch(TimeEntry.Functions.filterHourType);
    },

    filterHourType: function (executionContext) {

        var formContext = executionContext.getFormContext();
        //var hourtype = formContext.getAttribute("smt_lp_type_hours").getValue();
        var resourceId = formContext.getAttribute("msdyn_bookableresource").getValue()[0].id.replace("{", "").replace("}", "");
        var organizationalunitId = TimeEntry.Functions.GetResourceOrganizationalUnit(resourceId);
        var typehoursFilter = "<filter type='and'>" +
            "<condition value='1' attribute='statuscode' operator='eq' />" +
            "<condition value='" + organizationalunitId + "' attribute='smt_lp_organizationunit' operator='eq' />" +
            "</filter>";

        formContext.getControl("smt_lp_type_hours").addCustomFilter(typehoursFilter);
    },

    validateParameter: function (executionContext) {
        var formContext = executionContext.getFormContext();

        if (formContext.getAttribute("msdyn_entrystatus").getValue() === 192350002 || formContext.getAttribute("msdyn_entrystatus").getValue() === "192350004") {
            formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("none");
            formContext.getAttribute("smt_db_hours_left").setRequiredLevel("none");
            formContext.getControl("smt_dc_task_percentage").setDisabled(true);
            formContext.getControl("smt_db_hours_left").setDisabled(true);
            formContext.getAttribute("smt_db_hours_left").setValue(null);
            formContext.getAttribute("smt_dc_task_percentage").setValue(null);
        }
        else {
        var idProject = formContext.getAttribute("msdyn_project").getValue();//[0].id

            if (idProject === null) {
                formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("none");
                formContext.getAttribute("smt_db_hours_left").setRequiredLevel("none");
                formContext.getControl("smt_dc_task_percentage").setDisabled(true);
                formContext.getControl("smt_db_hours_left").setDisabled(true);
                formContext.getAttribute("smt_db_hours_left").setValue(null);
                formContext.getAttribute("smt_dc_task_percentage").setValue(null);
            } else {
                idProject = idProject[0].id;
                idProject = (idProject.replace("}", "")).replace("{", "");

                var idUnidadeOrganizacional = TimeEntry.Functions.GetUnidadeOrganizacional(idProject);
                var parameter = TimeEntry.Functions.GetParameter("ENABLE%20TASK%20PROGRESS%20UPDATE", idUnidadeOrganizacional);

                formContext.getAttribute("smt_db_hours_left").setRequiredLevel("none");
                formContext.getControl("smt_db_hours_left").setDisabled(true);

                if (parameter.value.length > 0) {
                    var continua = false;
                    for (i = 0; i < parameter.value.length; i++) {
                        if (continua === false) {
                            if (parameter.value[i]._smt_organizationalunit_value === idUnidadeOrganizacional && parameter.value[i]["smt_value"] === "true") {
                                formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("required");
                                formContext.getControl("smt_dc_task_percentage").setDisabled(false);
                                continua = true;
                            }
                            else if ((parameter.value[i]._smt_organizationalunit_value === "" || parameter.value[i]._smt_organizationalunit_value === null) && parameter.value[i]["smt_value"] === "true") {
                                formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("required");
                                formContext.getControl("smt_dc_task_percentage").setDisabled(false);
                                continua = true;
                            }
                            else {
                                formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("none");
                                formContext.getControl("smt_dc_task_percentage").setDisabled(true);
                            }
                        }
                    }
                }
            }
        }
    },

    GetResourceOrganizationalUnit: function (resourceId_) {

        var results = { isFault: false, data: [], erro: null };
        var result = "";
        var idUnidadeOrganizacional = "";
        var globalContext = Xrm.Utility.getGlobalContext();

        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/bookableresources(" + resourceId_ + ")?$select=_msdyn_organizationalunit_value", false);
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
                    idUnidadeOrganizacional = result["_msdyn_organizationalunit_value"];
                } else {
                    TimeEntry.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();
        return idUnidadeOrganizacional
    },

    GetUnidadeOrganizacional: function (idProject) {

        var results = { isFault: false, data: [], erro: null };
        var _result = "";
        var idUnidadeOrganizacional = "";
        var globalContext = Xrm.Utility.getGlobalContext();

        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/msdyn_projects?$select=_msdyn_contractorganizationalunitid_value&$filter=msdyn_projectid eq " + idProject + "", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        idUnidadeOrganizacional = results.value[i]["_msdyn_contractorganizationalunitid_value"];
                        var _msdyn_contractorganizationalunitid_value_formatted = results.value[i]["_msdyn_contractorganizationalunitid_value@OData.Community.Display.V1.FormattedValue"];
                        var _msdyn_contractorganizationalunitid_value_lookuplogicalname = results.value[i]["_msdyn_contractorganizationalunitid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                    }
                } else {
                    TimeEntry.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();
        return idUnidadeOrganizacional
    },

    GetParameter: function (name, idBu) {
        var results = { isFault: false, data: [], erro: null };
        var globalContext = Xrm.Utility.getGlobalContext();

        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/smt_smartparameters?$select=smt_name,_smt_organizationalunit_value,smt_value&$filter=smt_name eq '" + name + "'", false);
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
                        var smt_name = results.value[i]["smt_name"];
                        var _smt_organizationalunit_value = results.value[i]["_smt_organizationalunit_value"];
                        var _smt_organizationalunit_value_formatted = results.value[i]["_smt_organizationalunit_value@OData.Community.Display.V1.FormattedValue"];
                        var _smt_organizationalunit_value_lookuplogicalname = results.value[i]["_smt_organizationalunit_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                        var smt_value = results.value[i]["smt_value"];
                    }
                } else {
                    TimeEntry.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();
        return results;
    },

    AtualizaTarefaProjeto: function (executionContext) {
        var formContext = executionContext.getFormContext();

        var idProject = formContext.getAttribute("msdyn_project").getValue()[0].id
        idProject = (idProject.replace("}", "")).replace("{", "");

        var idUnidadeOrganizacional = GetUnidadeOrganizacional(idProject);
        var parameter = GetParameter(idUnidadeOrganizacional);

        if (parameter.value.length > 0) {

            if (parameter.value[0]["smt_value"] !== "true") {
                return;
            } else if (parameter.value[0]["_smt_organizationalunit_value"] === idUnidadeOrganizacional || parameter.value[0]["_smt_organizationalunit_value"] === "" || parameter.value[0]["_smt_organizationalunit_value"] === null) {

                var entradaHoraId = formContext.data.entity.getId();//formContext.getAttribute("msdyn_projecttask").getValue()[0].id;
                entradaHoraId = (entradaHoraId.replace("}", "")).replace("{", "");

                var PercentualDaTarefa = formContext.getAttribute("smt_dc_task_percentage").getValue();
                var HorasRestantes = formContext.getAttribute("smt_db_hours_left").getValue();

                var StatusEntrada = formContext.getAttribute("msdyn_entrystatus").getValue();

                if (StatusEntrada === 192350002) {
                    TimeEntry.Functions.ActionUpdateTarefaProjeto(entradaHoraId, PercentualDaTarefa, HorasRestantes);
                }
            }
        }
    },


    ActionUpdateTarefaProjeto: function (entradaHoraId, Percentual, Horas) {
        var parameters = {};
        var entity = {};
        entity.id = "" + entradaHoraId + "";
        entity.entityType = "msdyn_timeentry";
        parameters.entity = entity;
        parameters.PercentualDaTarefa = Percentual;
        parameters.HorasRestantes = Horas;

        var smt_AC_Update_Tarefa_do_ProjetoRequest = {
            entity: parameters.entity,
            PercentualDaTarefa: parameters.PercentualDaTarefa,
            HorasRestantes: parameters.HorasRestantes,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.msdyn_timeentry",
                            "structuralProperty": 5
                        },
                        "PercentualDaTarefa": {
                            "typeName": "Edm.Decimal",
                            "structuralProperty": 1
                        },
                        "HorasRestantes": {
                            "typeName": "Edm.Double",
                            "structuralProperty": 1
                        }
                    },
                    operationType: 0,
                    operationName: "smt_AC_Update_Tarefa_do_Projeto"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_Update_Tarefa_do_ProjetoRequest).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                }
            },
            function (error) {
                //Xrm.Utility.alertDialog(error.message);
                TimeEntry.Functions.showAlertDialog(error.message);
            }
        );

    },

    showAlertDialog: function (message_) {
        var message = { confirmButtonLabel: "OK", text: message_ };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {
                //console.log("Alert dialog closed");
            },
            function (error) {
                //console.log(error.message);
            })
    },

    // Define, por padrão, o tipo de hora como "Normal". Caso não haja este tipo de hora na entidade de mesmo nome,
    // o tipo de hora virá nulo por padrão. 
    setDefaultHourType: function (executionContext) {
        // debugger;
        // Declaração de variaveis. 
        var formContext = executionContext.getFormContext();
        var valOptionSet = "100000000"; // Refere-se ao valor da opção "Normal" no picklist smt_pl_type_hours.
        var pl = TimeEntry.Functions.retrieveHourType(valOptionSet);

        if (pl !== null && pl !== undefined && pl.value.length > 0) {
            var lookup = new Array();
            lookup[0] = new Object();
            lookup[0].id = pl.value[0].smt_type_hoursid;
            lookup[0].name = pl.value[0].smt_name;
            lookup[0].entityType = "smt_type_hours";
            formContext.getAttribute("smt_lp_type_hours").setValue(lookup);
        }
    },

    retrieveHourType: function (valOptionSet) {

        var results = null;
        var globalContext = Xrm.Utility.getGlobalContext();
        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/smt_type_hourses?$select=smt_pl_type_hours,smt_name&$filter=smt_pl_type_hours eq " + valOptionSet + "", false);
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
                        var smt_pl_type_hours = results.value[i]["smt_pl_type_hours"];
                        var smt_name = results.value[i]["smt_name"];
                        var smt_pl_type_hours_formatted = results.value[i]["smt_pl_type_hours@OData.Community.Display.V1.FormattedValue"];
                    }
                } else {
                    TimeEntry.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;

    },

    hideOptionClassification: function () {
        if (Xrm.Page.getAttribute("smt_pl_hours_classification") != null && Xrm.Page.getAttribute("smt_pl_hours_classification").getValue() != 180580009 && Xrm.Page.getAttribute("smt_pl_hours_classification").getValue() != 180580010) {
            Xrm.Page.getControl("smt_pl_hours_classification").removeOption(180580009);
            Xrm.Page.getControl("smt_pl_hours_classification").removeOption(180580010);
        }
    }


}

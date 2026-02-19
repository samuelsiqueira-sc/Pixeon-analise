// Nome da entidade: Entrada de Hora
if (typeof (TimeEntry) === undefined || typeof (TimeEntry) === "undefined") { TimeEntry = {}; }

var TimeEntry_LogicalName = "smt_purchasing_period";
TimeEntry.Functions = {
    fillFieldsBasedTask: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        if (formContext.getAttribute("msdyn_projecttask").getValue() !== null) {

            var idTask = formContext.getAttribute("msdyn_projecttask").getValue()[0].id;

            var retrieve = TimeEntry.Functions.retrieveFieldsProjectTask(idTask.replace("{", "").replace("}", ""), formContext);
        }
        else {
            formContext.getAttribute("smt_db_hours_left").setValue(null);
            // formContext.getAttribute("smt_dc_task_percentage").setValue(null);
        }
    },

    retrieveFieldsProjectTask: function (_id, formContext) {
        "use strict";
        var globalContext = Xrm.Utility.getGlobalContext();
        var result = { isFault: false, data: null, erro: null };

        Xrm.WebApi.online.retrieveRecord("msdyn_projecttask", _id, "?$select=msdyn_remaininghours,smt_progressofisico").then(
            function success(response) {
                result.data = {
                    msdyn_remaininghours: response["msdyn_remaininghours"],
                    msdyn_remaininghours_formatted: response["msdyn_remaininghours@OData.Community.Display.V1.FormattedValue"],
                    smt_progressofisico: response["smt_progressofisico"],
                    smt_progressofisico_formatted: response["smt_progressofisico@OData.Community.Display.V1.FormattedValue"]
                }

                if (result !== null && result.data !== null) {
                    var horasRestantes = 0;
                    if (result.data.msdyn_remaininghours !== null) {
                        horasRestantes = result.data.msdyn_remaininghours;

                    }
                    var percentual = 0;
                    if (result.data.smt_progressofisico !== null) {
                        percentual = result.data.smt_progressofisico;

                    }

                    formContext.getAttribute("smt_db_hours_left").setValue(horasRestantes);
                    formContext.getAttribute("smt_dc_task_percentage").setValue(percentual);
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    setFilter: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        formContext.getControl("msdyn_projecttask").addPreSearch(TimeEntry.Functions.filterProjectTask);
    },

    filterProjectTask: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        var projecttask = formContext.getAttribute("msdyn_projecttask").getValue();
        var actualDate = new Date().toISOString().slice(0, 10).replace("-", "/").replace("-", "/");
        var timeEntryFilter = "<filter type='and'>" +
            "<condition value='1' attribute='statuscode' operator='eq' />" +
            "<condition value='100000002' attribute='smt_statusprojeto' operator='ne' />" +
            "<condition value='" + actualDate + "' attribute='msdyn_scheduledstart' operator='on-or-before' />" +
            "</filter>";

        formContext.getControl("msdyn_projecttask").addCustomFilter(timeEntryFilter);
    },

    setFilterHourType: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        formContext.getControl("smt_lp_type_hours").addPreSearch(TimeEntry.Functions.filterHourType);
    },

    validateParameter: function (executionContext) {
        "use strict";
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

                var idUnidadeOrganizacional = TimeEntry.Functions.GetUnidadeOrganizacionalParameter(idProject, formContext);
            }
        }
    },

    GetUnidadeOrganizacionalParameter: function (idProject, formContext) {
        "use strict";
        var idUnidadeOrganizacional = "";
        var resultsUnit = { isFault: false, data: [], erro: null };
        var resultsParameters = { isFault: false, data: [], erro: null };

        Xrm.WebApi.online.retrieveMultipleRecords("msdyn_project", "?$select=_msdyn_contractorganizationalunitid_value&$filter=msdyn_projectid eq " + idProject).then(
            function success(results) {
                for (var i = 0; i < results.entities.length; i++) {
                    idUnidadeOrganizacional = results.entities[i]["_msdyn_contractorganizationalunitid_value"];
                    resultsUnit.data[i] = {
                        _msdyn_contractorganizationalunitid_value_formatted: results.entities[i]["_msdyn_contractorganizationalunitid_value@OData.Community.Display.V1.FormattedValue"],
                        _msdyn_contractorganizationalunitid_value_lookuplogicalname: results.entities[i]["_msdyn_contractorganizationalunitid_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
                    }
                }
                if (idUnidadeOrganizacional !== null) {

                    var nameParameter = "ENABLE%20TASK%20PROGRESS%20UPDATE";
                    Xrm.WebApi.online.retrieveMultipleRecords("smt_smartparameter", "?$select=smt_name,_smt_organizationalunit_value,smt_value&$filter=smt_name eq '" + nameParameter + "'").then(
                        function success(parameter) {
                            for (var i = 0; i < parameter.entities.length; i++) {
                                resultsParameters.data[i] = {
                                    smt_name: parameter.entities[i]["smt_name"],
                                    _smt_organizationalunit_value: parameter.entities[i]["_smt_organizationalunit_value"],
                                    _smt_organizationalunit_value_formatted: parameter.entities[i]["_smt_organizationalunit_value@OData.Community.Display.V1.FormattedValue"],
                                    _smt_organizationalunit_value_lookuplogicalname: parameter.entities[i]["_smt_organizationalunit_value@Microsoft.Dynamics.CRM.lookuplogicalname"],
                                    smt_value: parameter.entities[i]["smt_value"]
                                }
                            }

                            formContext.getAttribute("smt_db_hours_left").setRequiredLevel("none");
                            formContext.getControl("smt_db_hours_left").setDisabled(true);

                            if (resultsParameters !== null && resultsParameters.data !== null && resultsParameters.data.length > 0) {
                                var continua = false;
                                for (i = 0; i < resultsParameters.data.length; i++) {
                                    if (continua === false) {
                                        if (resultsParameters.data[i]._smt_organizationalunit_value === idUnidadeOrganizacional && resultsParameters.data[i].smt_value === "true") {
                                            formContext.getAttribute("smt_dc_task_percentage").setRequiredLevel("required");
                                            formContext.getControl("smt_dc_task_percentage").setDisabled(false);
                                            continua = true;
                                        }
                                        else if ((resultsParameters.data[i]._smt_organizationalunit_value === "" || resultsParameters.data[i]._smt_organizationalunit_value === null) && resultsParameters.data[i].smt_value === "true") {
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
                        },
                        function (error) {
                            Xrm.Navigation.openAlertDialog(error.message);
                        }
                    );
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    // Define, por padrão, o tipo de hora como "Normal". Caso não haja este tipo de hora na entidade de mesmo nome, o tipo de hora virá nulo por padrão. 
    SetDefaultHourType: function (executionContext) {
        "use strict";
        // Declaração de variaveis. 
        var formContext = executionContext.getFormContext();
        var valOptionSet = "100000000"; // Refere-se ao valor da opção "Normal" no picklist smt_pl_type_hours.
        var pl = TimeEntry.Functions.retrieveHourType(valOptionSet, formContext);
    },

    // Define, por padrão, o tipo de hora como "Normal". Caso não haja este tipo de hora na entidade de mesmo nome, o tipo de hora virá nulo por padrão. 
    SetDefaultHourTypeOnChange: function (executionContext) {
        "use strict";
        // Declaração de variaveis. 
        var formContext = executionContext.getFormContext();
        if (formContext.getAttribute("msdyn_type").getValue() === "192350000") {
            var valOptionSet = "100000000"; // Refere-se ao valor da opção "Normal" no picklist smt_pl_type_hours.
            var pl = TimeEntry.Functions.retrieveHourType(valOptionSet, formContext);
        }
    },

    ActionUpdateTarefaProjeto: function (entradaHoraId, Percentual, Horas) {
        "use strict";
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
    },

    retrieveHourType: function (valOptionSet, formContext) {
        "use strict";
        var results = { isFault: false, data: [], erro: null };

        Xrm.WebApi.online.retrieveMultipleRecords("smt_type_hours", "?$select=smt_name,smt_pl_type_hours&$filter=smt_pl_type_hours eq " + valOptionSet).then(
            function success(result) {
                for (var i = 0; i < result.entities.length; i++) {
                    results.data[i] = {
                        smt_name: result.entities[i]["smt_name"],
                        smt_pl_type_hours: result.entities[i]["smt_pl_type_hours"],
                        smt_pl_type_hours_formatted: result.entities[i]["smt_pl_type_hours@OData.Community.Display.V1.FormattedValue"],
                        smt_type_hoursid: result.entities[i]["smt_type_hoursid"]
                    }
                }

                if (results !== null && results !== undefined && results.data.length > 0) {
                    var lookup = new Array();
                    lookup[0] = new Object();
                    lookup[0].id = results.data[0].smt_type_hoursid;
                    lookup[0].name = results.data[0].smt_name;
                    lookup[0].entityType = "smt_type_hours";
                    formContext.getAttribute("smt_lp_type_hours").setValue(lookup);
                }
            },
            function (error) {
                TimeEntry.Functions.showAlertDialog(this.statusText);
            }
        );
    },


    OnSave: function (executionContext) {
        "use strict";
        //debugger;
        var formContext = executionContext.getFormContext();

        if (!formContext.getAttribute("smt_pl_save").getValue()) {
            if (formContext.getAttribute("smt_dc_task_percentage").getValue() === null || formContext.getAttribute("smt_dc_task_percentage").getValue() === 0) {
                executionContext.getEventArgs().preventDefault();
                var confirmStrings = { text: "", title: "Alerta Porcentagem", subtitle: "A porcentagem da entrada de horas não bate com a quantidade de horas cadastrada, deseja realmente enviar?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
                var confirmOptions = { height: 200, width: 500 };
                Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                    function (success) {
                        //debugger;
                        if (success.confirmed) {
                            formContext.getAttribute("smt_pl_save").setValue(true);
                            formContext.data.save();
                            // formContext.ui.close();
                        }
                        else
                            executionContext.getEventArgs().preventDefault();
                    });
            }
            else {
                var result = null;

                Xrm.WebApi.retrieveRecord("msdyn_projecttask", formContext.getAttribute("msdyn_projecttask").getValue()[0].id.replace("{", "").replace("}", ""), "?$select=smt_progressofisico").then(
                    function success(result) {
                       // console.log(result);
                        // Columns
                        var msdyn_projecttaskid = result["msdyn_projecttaskid"]; // Guid
                        var smt_progressofisico = result["smt_progressofisico"]; // Decimal
                        var smt_progressofisico_formatted = result["smt_progressofisico@OData.Community.Display.V1.FormattedValue"];
                    },
                    function (error) {
                        ///console.log(error.message);
                    }
                );
                /*
                var req = new XMLHttpRequest();
                req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/msdyn_projecttasks(" + formContext.getAttribute("msdyn_projecttask").getValue()[0].id.replace("{", "").replace("}", "") + ")?$select=smt_progressofisico", false);
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
                            var smt_progressofisico = result["smt_progressofisico"];
                            var smt_progressofisico_formatted = result["smt_progressofisico@OData.Community.Display.V1.FormattedValue"];
                        } else {
                            Xrm.Utility.alertDialog(this.statusText);
                        }
                    }
                };
                req.send();
                */
                if (result !== null) {
                    if (result.smt_progressofisico > formContext.getAttribute("smt_dc_task_percentage").getValue()) {
                        executionContext.getEventArgs().preventDefault();
                        var confirmStrings = { text: "", title: "Alerta Porcentagem", subtitle: "A porcentagem da entrada de horas não bate com o progresso fisico da tarefa, deseja realmente enviar?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
                        var confirmOptions = { height: 200, width: 500 };
                        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                            function (success) {
                                //debugger;
                                if (success.confirmed) {
                                    formContext.getAttribute("smt_pl_save").setValue(true);
                                    formContext.data.save();
                                    // formContext.ui.close();
                                }
                                else
                                    executionContext.getEventArgs().preventDefault();
                            });
                    }
                    else {
                        var results = null;
                        Xrm.WebApi.retrieveMultipleRecords("msdyn_timeentry", "?$filter=(smt_dc_task_percentage gt " + formContext.getAttribute("smt_dc_task_percentage").getValue() + "  and msdyn_targetentrystatus eq 192350003 and _msdyn_projecttask_value eq " + formContext.getAttribute("msdyn_projecttask").getValue()[0].id.replace("{", "").replace("}", "") + ")").then(
                            function success(results) {
                               // console.log(results);
                                for (var i = 0; i < results.entities.length; i++) {
                                    var result = results.entities[i];
                                    // Columns
                                    var msdyn_timeentryid = result["msdyn_timeentryid"]; // Guid
                                }
                            },
                            function (error) {
                              //  console.log(error.message);
                            }
                        );
                        /*
                        var reqs = new XMLHttpRequest();
                        reqs.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/msdyn_timeentries?$filter=smt_dc_task_percentage gt " + formContext.getAttribute("smt_dc_task_percentage").getValue() + " and msdyn_targetentrystatus eq 192350003 and _msdyn_projecttask_value eq " + formContext.getAttribute("msdyn_projecttask").getValue()[0].id.replace("{", "").replace("}", ""), false);
                        reqs.setRequestHeader("OData-MaxVersion", "4.0");
                        reqs.setRequestHeader("OData-Version", "4.0");
                        reqs.setRequestHeader("Accept", "application/json");
                        reqs.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                        reqs.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
                        reqs.onreadystatechange = function () {
                            if (this.readyState === 4) {
                                reqs.onreadystatechange = null;
                                if (this.status === 200) {
                                    results = JSON.parse(this.response);
                                    for (var i = 0; i < results.value.length; i++) {
                                        var msdyn_timeentryid = results.value[i]["msdyn_timeentryid"];
                                    }
                                } else {
                                    Xrm.Utility.alertDialog(this.statusText);
                                }
                            }
                        };
                        reqs.send();
                        */
                        if (results !== undefined && results !== null && results.value.length === 0 && formContext.getAttribute("smt_dc_task_percentage").getValue() === 0) {
                            executionContext.getEventArgs().preventDefault();
                            var confirmStrings = { text: "", title: "Alerta Porcentagem", subtitle: "A porcentagem da entrada de horas não bate com o progresso fisico da tarefa, deseja realmente enviar?", "cancelButtonLabel": "NÃO", confirmButtonLabel: "SIM" };
                            var confirmOptions = { height: 200, width: 500 };
                            Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                                function (success) {
                                    //debugger;
                                    if (success.confirmed) {
                                        formContext.getAttribute("smt_pl_save").setValue(true);
                                        formContext.data.save();
                                        // formContext.ui.close();
                                    }
                                    else
                                        executionContext.getEventArgs().preventDefault();
                                });
                        }
                    }
                }
            }
        }
    }
}
if (typeof (salesOrder) === undefined || typeof (salesOrder) === "undefined") { salesOrder = {}; }

var salesOrder_LogicalName = "salesorder";

salesOrder.Functions = {

    //openCreateProject: function (primaryControl) {
    //	var formContext = primaryControl;
    //	var entityName_ = formContext.data.entity.getEntityName();
    //	var _id = formContext.data.entity.getId();
    //	_id = _id.replace("{", "").replace("}", "");

    //	var entityFormOptions = {};
    //	entityFormOptions["entityName"] = entityName_;
    //	entityFormOptions["useQuickCreateForm"] = true;

    //	var formParameters = {};

    //	// Open the form.
    //	Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
    //		function (success) {
    //			console.log(success);
    //		},
    //		function (error) {
    //			console.log(error);
    //		});
    //},

    autoCompleteUri: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        var pl_azure_devops_structure = formContext.getAttribute("smt_pl_azure_devops_structure").getValue();

        if (pl_azure_devops_structure === 100000001) {

            var project_name = formContext.getAttribute("smt_st_project_name").getValue();

            if (formContext.getAttribute("smt_lp_azure_devops_settings").getValue() !== null) {

                var azure_devops_structure = formContext.getAttribute("smt_lp_azure_devops_settings").getValue()[0].id;
                azure_devops_structure = azure_devops_structure.replace("{", "").replace("}", "");

                var azure = salesOrder.Functions.getDevOpsSettings(azure_devops_structure, project_name, formContext);

                //if (azure.smt_st_organization_uri == null) {
                //    var alertStrings = { confirmButtonLabel: "OK", text: "A configuração do Azure DevOps não contém uma uri preenchida!" };
                //    var alertOptions = { height: 120, width: 260 };
                //    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                //        function success(result) {
                //            //console.log("Alert dialog closed");
                //        },
                //        function (error) {
                //            //concole.log(error.message);
                //        }
                //    );
                //}
                //else {

                //    var encodedValue = encodeURIComponent(project_name);

                //    var result = azure.smt_st_organization_uri + "_apis/projects/" + encodedValue;

                //    formContext.getAttribute("smt_st_azure_devops_uri").setValue(result);

                //}
            }
            else {
                var alertStrings = { confirmButtonLabel: "OK", text: "É preciso preencher o campo 'Configuração do Azure DevOps' primeiro." };
                var alertOptions = { height: 120, width: 260 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function success(result) {
                        //console.log("Alert dialog closed");
                    },
                    function (error) {
                        //concole.log(error.message);
                    }
                );
            }
        }
    },

    getDevOpsSettings: function (id, project_name, formContext) {
        "use strict";
        var globalContext = Xrm.Utility.getGlobalContext();
        var result = null;

        Xrm.WebApi.online.retrieveRecord("smt_azure_devops_settings", id, "?$select=smt_st_organization_uri").then(
            function success(result) {
                var smt_st_organization_uri = result["smt_st_organization_uri"];
                if (smt_st_organization_uri === null) {
                    var alertStrings = { confirmButtonLabel: "OK", text: "A configuração do Azure DevOps não contém uma uri preenchida!" };
                    var alertOptions = { height: 120, width: 260 };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                        function success(result) {
                            //console.log("Alert dialog closed");
                        },
                        function (error) {
                            //concole.log(error.message);
                        }
                    );
                }
                else {

                    var encodedValue = encodeURIComponent(project_name);

                    result = smt_st_organization_uri + "_apis/projects/" + encodedValue;

                    formContext.getAttribute("smt_st_azure_devops_uri").setValue(result);

                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
        //var req = new XMLHttpRequest();
        //req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/smt_azure_devops_settingses(" + id + ")?$select=smt_st_organization_uri", false);
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
        //            var smt_st_organization_uri = result["smt_st_organization_uri"];
        //        } else {
        //            salesOrder.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();

        return result;
    },

    unlinkProject: function (primaryControl) {
        "use strict";
        //debugger;

        var guid = primaryControl.data.entity.getId();
        guid = guid.replace("{", "").replace("}", "");

        var confirmString = { text: "", title: "Desvincular Projeto", subtitle: "Deseja realmente desvincular esse projeto do Azure?", cancelButtonLabel: "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 150, width: 300 };
        Xrm.Navigation.openConfirmDialog(confirmString, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var projects = salesOrder.Functions.retrieveProject(guid);
                    //for (var i = 0; i < projects.value.length; i++) {
                    //    if (projects.value[i].msdyn_project_value != null) {
                    //        salesOrder.Functions.actionProject(projects.value[i]._msdyn_project_value);
                    //    }
                    //}

                    //salesOrder.Functions.actionOrder(guid);
                    //primaryControl.data.refresh();

                }
            }
        );
    },

    retrieveProject: function (guid) {
        "use strict";
        // debugger;
        var results = null;

        Xrm.WebApi.online.retrieveMultipleRecords("salesorderdetail", "?$select=_msdyn_project_value&$filter=_salesorderid_value eq " + guid).then(
            function success(results) {
                for (var i = 0; i < results.entities.length; i++) {
                    var _msdyn_project_value = results.entities[i]["_msdyn_project_value"];
                    var _msdyn_project_value_formatted = results.entities[i]["_msdyn_project_value@OData.Community.Display.V1.FormattedValue"];
                    var _msdyn_project_value_lookuplogicalname = results.entities[i]["_msdyn_project_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

                    if (_msdyn_project_value !== null) {
                        salesOrder.Functions.actionProject(_msdyn_project_value);
                    }
                }
                salesOrder.Functions.actionOrder(guid);
                primaryControl.data.refresh();
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );

        //var results = null;
        //var req = new XMLHttpRequest();
        //req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/salesorderdetails?$select=_msdyn_project_value&$filter=_salesorderid_value eq " + guid + "", false);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 200) {
        //            results = JSON.parse(this.response);
        //            for (var i = 0; i < results.value.length; i++) {
        //                var _msdyn_project_value = results.value[i]["_msdyn_project_value"];
        //                var _msdyn_project_value_formatted = results.value[i]["_msdyn_project_value@OData.Community.Display.V1.FormattedValue"];
        //                var _msdyn_project_value_lookuplogicalname = results.value[i]["_msdyn_project_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
        //            }
        //        } else {
        //            salesOrder.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();

        return results;
    },

    actionProject: function (id) {
        "use strict";
        //debugger;

        id = id.replace("{", "").replace("}", "");
        var parameters = {};
        var entity = {};
        entity.id = id;
        entity.entityType = "msdyn_project";
        parameters.entity = entity;

        var smt_AC_CleanFieldinProjectRequest = {
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
                    operationName: "smt_AC_CleanFieldinProject"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_CleanFieldinProjectRequest).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
        //var req = new XMLHttpRequest();
        //req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/msdyn_projects(" + id + ")/Microsoft.Dynamics.CRM.smt_AC_CleanFieldinProject", true);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 204) {
        //            //Success - No Return Data - Do Something
        //        } else {
        //            salesOrder.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();
    },

    actionOrder: function (guid) {
        "use strict";
        // debugger;

        var parameters = {};
        var entity = {};
        entity.id = guid;
        entity.entityType = "salesorder";
        parameters.entity = entity;

        var smt_AC_UnlinkProjectRequest = {
            entity: parameters.entity,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.salesorder",
                            "structuralProperty": 5
                        }
                    },
                    operationType: 0,
                    operationName: "smt_AC_UnlinkProject"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_UnlinkProjectRequest).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
        //var req = new XMLHttpRequest();
        //req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/salesorders(" + guid + ")/Microsoft.Dynamics.CRM.smt_AC_UnlinkProject", false);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 204) {
        //            //Success - No Return Data - Do Something
        //        } else {
        //            salesOrder.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();
    },

    visibleUnlink: function (primaryControl) {
        "use strict";
        var showButton = false;

        // var formcontext = primaryControl.getFormContext();
        var guid = primaryControl.data.entity.getId();
        guid = guid.replace("{", "").replace("}", "");

        if (primaryControl.getAttribute("smt_pl_azure_devops_structure").getValue() !== null) {
            if (primaryControl.getAttribute("smt_pl_azure_devops_structure").getValue() !== 100000002) {
                showButton = true;
            }
        }
        return showButton;
    },

    refreshPage: function (executionContext) {
        "use strict";
        //debugger;
        var formContext = executionContext.getFormContext();
        formContext.ui.refreshRibbon();
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
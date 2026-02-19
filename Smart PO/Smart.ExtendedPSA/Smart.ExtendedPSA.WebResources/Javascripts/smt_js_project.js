if (typeof (Project) === undefined || typeof (Project) === "undefined") { Project = {}; }

var Project_LogicalName = "msdyn_project";
Project.Functions = {

    updateIteration: function (primaryControl) {
        //debugger;
        "use strict";
        var areapath = primaryControl.getAttribute("smt_st_area_path").getValue();
        var actualstart = primaryControl.getAttribute("msdyn_scheduledstart").getValue();
        var actualend = primaryControl.getAttribute("msdyn_scheduledend").getValue();


        if ((actualstart === null && actualstart === undefined) || (actualend === null && actualend === undefined)) {

            var alertStrings = { confirmButtonLabel: "OK", text: "O projeto não tem a data de estimativa preenchida!" };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function success(result) {

                    //console.log("Alert dialog closed");
                },
                function (error) {
                    //console.log(error.message);
                }
            );

            return;
        }

        if (areapath !== null && areapath !== undefined) {
            areapath = areapath.replace("\\", "\\\\");
        } else {

            var alertStrings = { confirmButtonLabel: "OK", text: "O projeto não esta associado a uma Area Path" };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function success(result) {
                    // console.log("Alert dialog closed");
                },
                function (error) {
                    // console.log(error.message);
                }
            );

            return;
        }


        if (primaryControl.getAttribute("msdyn_salesorderid").getValue() !== null) {

            var orderId = primaryControl.getAttribute("msdyn_salesorderid").getValue()[0].id;
            orderId = orderId.replace("{", "").replace("}", "");

            var recordId = primaryControl.data.entity.getId();
            recordId = recordId.replace("{", "").replace("}", "");

            var results = Project.Functions.getOrder(orderId);

            if (results !== null) {
                //debugger;
                if (results.smt_lp_azure_devops_settings !== null || results.smt_lp_azure_devops_settings !== undefined) {
                    var organization_uri = results.smt_lp_azure_devops_settings.smt_st_organization_uri;
                    var servicetoken = results.smt_lp_azure_devops_settings.smt_st_service_token;
                    var projectname = results.smt_st_project_name;
                    var workitem_type_ = results.smt_lp_azure_devops_settings.smt_st_workitem_parent;
                    var workitem_child = results.smt_lp_azure_devops_settings.smt_st_workitem_child;
                    var uri_buscadeiterao = results.smt_lp_azure_devops_settings.smt_st_uri_buscadeiterao;
                } else {
                    this.showAlertDialog("Impossível atualizar a iteração pois o contrato do projeto não tem integração com o Azure DevOps.");
                }

            } else {
                return;
            }
        } else {

            var alertStrings = { confirmButtonLabel: "OK", text: "O projeto precisa estar associado a um Contrato!" };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function success(result) {
                    //console.log("Alert dialog closed");
                },
                function (error) {
                    //concole.log(error.message);
                }
            );

            return;
        }

        let integrationDate = "";

        if (primaryControl.getAttribute("smt_dt_integration").getValue() !== null && primaryControl.getAttribute("smt_dt_integration").getValue() !== undefined) {
            integrationDate = primaryControl.getAttribute("smt_dt_integration").getValue().toISOString();
        }

        var settings = {
            "async": true,
            "crossDomain": true,
            "url": uri_buscadeiterao,
            "method": "POST",
            "headers": {
                "Content-Type": "application/json",
                "cache-control": "no-cache",
                "Postman-Token": "4ed3681b-f0bf-4154-b805-5570500eb1d9"
            },
            "processData": false,
            "data": "{\n    \"url\": \"" + organization_uri + "\",\n    \"name\": \"" + projectname + "\",\n    \"token\": \"" + servicetoken + "\",\n    \"newAreaPath\": \"" + areapath + "\",\n    \"newIteration\": \"\",\n    \"smt_st_workitem_child\": \"" + workitem_child + "\",\n    \"_newIterationStart\": \"\",\n    \"_newIterationEnd\": \"\",\n    \"workitem_type\": \"" + workitem_type_ + "\",\n    \"integrationDate\":\"" + integrationDate + "\"\n}"
        }

        $.ajax(settings).done(function (response) {
        });

        var alertStrings = { confirmButtonLabel: "OK", text: "Foi enviada uma solicitação de atualização para o AzureDevOps! Aguarde alguns instantes..." };
        var alertOptions = { height: 120, width: 260 };
        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            function success(result) {

            },
            function (error) {

            }
        );
    },

    getOrder: async function (id) {
        "use strict";
        var globalContext = Xrm.Utility.getGlobalContext();
        var result = await Xrm.WebApi.retrieveMultipleRecords("salesorder", "?$select=smt_st_project_name&$expand=smt_lp_azure_devops_settings($select=smt_st_service_token,smt_st_uri_buscadeiterao,smt_st_organization_uri,smt_st_workitem_parent,smt_st_workitem_child)&$filter=salesorderid eq " + id).then(
            function success(results) {
                return results;
            },
            function (error) {
                return null
            }
        );
        return result;
        /* DEPRECIADO
        var result = null;
        var req = new XMLHttpRequest();
        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/salesorders(" + id + ")?$select=smt_st_project_name&$expand=smt_lp_azure_devops_settings($select=smt_st_organization_uri,smt_st_service_token,smt_st_workitem_parent,smt_st_workitem_child,smt_st_uri_buscadeiterao)", false);
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
                    var smt_st_project_name = result["smt_st_project_name"];
                    if (result.hasOwnProperty("smt_lp_azure_devops_settings")) {
                        var smt_lp_azure_devops_settings_smt_st_organization_uri = result["smt_lp_azure_devops_settings"]["smt_st_organization_uri"];
                        var smt_lp_azure_devops_settings_smt_st_service_token = result["smt_lp_azure_devops_settings"]["smt_st_service_token"];
                        var smt_lp_azure_devops_settings_smt_st_workitem_parent = result["smt_lp_azure_devops_settings"]["smt_st_workitem_parent"];
                        var smt_lp_azure_devops_settings_smt_st_workitem_child = result["smt_lp_azure_devops_settings"]["smt_st_workitem_child"];
                        var smt_lp_azure_devops_settings_smt_st_uri_buscadeiterao = result["smt_lp_azure_devops_settings"]["smt_st_uri_buscadeiterao"];

                    }
                } else {
                    Project.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();

        return result;
        */
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
    },

    VerifyFieldState: function (formContext) {
        //debugger;
        "use strict";
        var DevOps = formContext.getAttribute("smt_st_area_path").getValue();
        var StatusProject = formContext.getAttribute("statuscode").getValue();

        if (StatusProject === "100000000" || StatusProject === 100000000 || StatusProject === "100000003" || StatusProject === 100000003) {
            if (DevOps !== null)
                return true;
            else
                return false;
        }
    },

    RefreshRibbon: function (executionContext) {
        "use strict";
        formContext = executionContext.getFormContext();

        formContext.ui.refreshRibbon();
    }

}
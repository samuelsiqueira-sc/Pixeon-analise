if (typeof (salesOrder) === undefined || typeof (salesOrder) === "undefined") { salesOrder = {}; }

var salesOrder_LogicalName = "salesorder";
salesOrder.Functions = {

    unlinkProject: function (primaryControl) {

        //debugger;
        var formcontext = primaryControl.getFormContext();
        var guid = primaryControl.data.entity.getId();
        guid = guid.replace("{", "").replace("}", "");


        var confirmString = { text: "", title: "Desvincular Projeto", subtitle: "Deseja realmente desvincular esse projeto do Azure?", cancelButtonLabel: "NÃO", confirmButtonLabel: "SIM" };
        var confirmOptions = { height: 150, width: 300 };
        Xrm.Navigation.openConfirmDialog(confirmString, confirmOptions).then(
            function (success) {
                if (success.confirmed) {

                    var projects = salesOrderUnlink.retrieveProject(guid);

                    for (var i = 0; i < projects.value.length; i++) {
                        salesOrderUnlink.Functions.actionProject(projects.value[i]._msdyn_project_value);
                    }

                    salesOrderUnlink.actionOrder(guid);

                }
            }
        );
    },

    retrieveProject: function (guid) {
        //debugger;

        var results = null;
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/salesorderdetails?$select=_msdyn_project_value&$filter=_salesorderid_value eq " + guid + "", false);
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
                        var _msdyn_project_value = results.value[i]["_msdyn_project_value"];
                        var _msdyn_project_value_formatted = results.value[i]["_msdyn_project_value@OData.Community.Display.V1.FormattedValue"];
                        var _msdyn_project_value_lookuplogicalname = results.value[i]["_msdyn_project_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                    }
                } else {
                    salesOrderUnlink.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();

        return results;
    },

    actionProject: function (id) {
        //debugger;

        id = id.replace("{", "").replace("}", "");

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/msdyn_projects(" + id + ")/Microsoft.Dynamics.CRM.smt_AC_CleanFieldinProject", true);
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
                    salesOrderUnlink.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();
    },

    actionOrder: function (guid) {
        //debugger;
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/salesorders(" + guid + ")/Microsoft.Dynamics.CRM.smt_AC_UnlinkProject", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success - No Return Data - Do Something
                    Xrm.Navigation.openForm("salesorder", guid);

                } else {
                    salesOrderUnlink.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();
    },

    visibleUnlink: function (primaryControl) {
        var showButton = false;

        var formcontext = primaryControl.getFormContext();
        var guid = primaryControl.data.entity.getId();
        guid = guid.replace("{", "").replace("}", "");

        if (primaryControl.getAttribute("smt_pl_azure_devops_structure").getValue() != null) {
            if (primaryControl.getAttribute("smt_pl_azure_devops_structure").getValue() !== 100000002) {
                showButton = true;
            }
        }
        return showButton;
    },

    refreshPage: function (primaryControl) {
        var formcontext = primaryControl.getFormContext();
        formcontext.ui.refreshRibbon();
    },

    showAlertDialog: function (message_) {
        var message = { confirmButtonLabel: "OK", text: message_ };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {

            },
            function (error) {

            })
    }

}
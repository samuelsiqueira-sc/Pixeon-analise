function CallAction(executionContext) {
    var formContext = executionContext.getFormContext();
    var globalContext = Xrm.Utility.getGlobalContext();
    var req = new XMLHttpRequest();
    var currentRecordIdString = formContext.data.entity.getId();
    var currentRecordId = currentRecordIdString.replace("{", '').replace("}", '');
    req.open("POST", globalContext.getClientUrl() + "/api/data/v9.1/quotes(" + currentRecordId + ")/Microsoft.Dynamics.CRM.smt_AC_Create_Change_RequestRecord", true);
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
                showAlertDialog(this.statusText);
            }
        }
    };
    req.send();
}

function showAlertDialog(message) {
    var message = { confirmButtonLabel: "OK", text: message };
    var alertOptions = { height: 150, width: 280 };

    Xrm.Navigation.openAlertDialog(message, alertOptions).then(
        function success(result) {

        },
        function (error) {

        })
}
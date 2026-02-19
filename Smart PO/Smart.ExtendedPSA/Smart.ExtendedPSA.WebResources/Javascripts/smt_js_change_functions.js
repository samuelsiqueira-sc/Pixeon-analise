if (typeof (ChangeFunctions) === undefined || typeof (ChangeFunctions) === "undefined") { ChangeFunctions = {}; }

var ChangeFunctions_LogicalName = "smt_change_functions";
ChangeFunctions.Functions = {

    UnblockResource: function (executionContext) {
        "use strict"
        //debugger;
        var formContext = executionContext.getFormContext();
        var formType = formContext.ui.getFormType();
        if (formType === 1) {
            var lpResource = formContext.getControl("header_smt_lp_resourceid");
            lpResource.setDisabled(false);
        }
    }
}
function hideSection(executionContext) {
    "use strict";
    var formContext = executionContext.getFormContext();
    var secObj = formContext.ui.tabs.get("tab_4").sections.get("tab_4_section_5");
    secObj.setVisible(false);
}

function showHideSection(executionContext) {
    "use strict";
    var formContext = executionContext.getFormContext();
    var milestoneVal = formContext.getAttribute("smt_lp_milestone").getValue();
    var idMilestone = "";
    if (milestoneVal !== null) {
        idMilestone = formContext.getAttribute("smt_lp_milestone").getValue()[0].id;
    }

    if (idMilestone !== null && idMilestone.length > 0) {
        Xrm.WebApi.online.retrieveRecord("smt_milestone_project", idMilestone, "?$select=smt_bl_approval").then(
            function success(result) {
                var smt_bl_approval = result["smt_bl_approval"];
                var smt_bl_approval_formatted = result["smt_bl_approval@OData.Community.Display.V1.FormattedValue"];

                if (smt_bl_approval === true) {

                    var secObj = formContext.ui.tabs.get("tab_4").sections.get("tab_4_section_5");
                    secObj.setVisible(true);

                } else if (smt_bl_approval === false) {


                    var secObj = formContext.ui.tabs.get("tab_4").sections.get("tab_4_section_5");
                    secObj.setVisible(false);

                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    }
}
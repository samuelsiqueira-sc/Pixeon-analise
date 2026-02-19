if (typeof ProjectMilestone === undefined || typeof ProjectMilestone === "undefined") { ProjectMilestone = {}; }

var ProjectMilestone_LogicalName = "smt_milestone_project";
ProjectMilestone.Functions = {
    RevenueType: function (executionContext) {
        debugger;

        var formContext = executionContext.getFormContext();
        var revenueType = formContext.getAttribute("smt_pl_type").getValue(); // Valor do campo "Tipo de Receita".

        if (revenueType === 0) //Se for igual a recorrente. 
        {
            formContext.getAttribute("smt_dc_stage").setValue(null);
            formContext.getAttribute("smt_dc_stage").setRequiredLevel("none");
            formContext.getControl("smt_bl_login_password").setVisible(true);
            formContext.getAttribute("smt_bl_login_password").setRequiredLevel("required");
            formContext.getControl("smt_dc_stage").setVisible(false);
        }
        else if (revenueType === 1) // Se for igual a eventual. 
        {
            formContext.getAttribute("smt_bl_login_password").setRequiredLevel("none");
            formContext.getAttribute("smt_bl_login_password").setValue(null);
            formContext.getControl("smt_dc_stage").setVisible(true);
            formContext.getAttribute("smt_dc_stage").setRequiredLevel("required");
            formContext.getControl("smt_bl_login_password").setVisible(false);
        }
        else // Se for igual a eventual. 
        {
            formContext.getAttribute("smt_bl_login_password").setRequiredLevel("none");
            formContext.getAttribute("smt_bl_login_password").setValue(null);
            formContext.getControl("smt_dc_stage").setVisible(false);
            formContext.getAttribute("smt_dc_stage").setRequiredLevel("none");
            formContext.getControl("smt_bl_login_password").setVisible(false);
        }
    }
};


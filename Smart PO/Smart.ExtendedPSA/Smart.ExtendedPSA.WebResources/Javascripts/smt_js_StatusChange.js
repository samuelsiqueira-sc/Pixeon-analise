function LimparAprovador(executionContext) {
    "use strict";
    var formContext = executionContext.getFormContext();
    var stts = formContext.getAttribute("smt_statusprojeto").getValue();
    if (stts !== null) {
        if (stts === 100000003) {
            formContext.getAttribute("smt_lp_approvedby").setValue("");
            formContext.getAttribute("smt_lp_reprovado_por").setValue("");
            formContext.getAttribute("smt_pl_status_aprovacao").setValue("");
            formContext.getAttribute("smt_lp_aprovador").setValue("");
            formContext.getAttribute("smt_pl_acceptance").setValue(0);
            formContext.getAttribute("smt_st_justificativa").setValue("");
        }
    }
}
function VerificarCampo(executionContext) {
    "use strict";
    //debugger;
    Form = executionContext.getFormContext();
    var team = Form.getAttribute("smt_bl_provider").getValue();
    var teste = Form.ui.tabs.get("SUMMARY_TAB").sections.get("Sec_Fornecedor");
    if (team === false) {
        teste.setVisible(false);
    } else {
        teste.setVisible(true);
    }
}
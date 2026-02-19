if (typeof (ChangeRequest) === undefined || typeof (ChangeRequest) === "undefined") { ChangeRequest = {}; }

var ChangeRequest_LogicalName = "quote";

ChangeRequest.Functions = {

    CreateChangeRequest: function (primaryControl) {
        "use strict";
        //debugger;
        var formContext = primaryControl;
        var globalContext = Xrm.Utility.getGlobalContext();
        var contratoContext = formContext.getAttribute("smt_lp_contract_main") !== null ? formContext.getControl("smt_lp_contract_main").getAttribute().getValue() : null;

        if (contratoContext === null || contratoContext === undefined) {

            ChangeRequest.Functions.Mensagem("Os campos Contrato Aditivo e Contrato Principal devem estar preenchidos.");
            return;
        }
        var idContrato = contratoContext[0].id;
        idContrato = (idContrato.replace("}", "")).replace("{", "");

        var idQuote = formContext.data.entity.getId();
        idQuote = (idQuote.replace("}", "")).replace("{", "");

        var parameters = {};
        parameters.Contrato = idContrato;

        var req = new XMLHttpRequest();
        req.open("POST", globalContext.getClientUrl() + "/api/data/v9.1/quotes(" + idQuote + ")/Microsoft.Dynamics.CRM.smt_AC_Gerar_Aditivo_Contrato", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    ChangeRequest.Functions.Mensagem("Aditivo Gerado com sucesso!");
                    formContext.data.refresh();
                    //Success - No Return Data - Do Something

                } else {
                    ChangeRequest.Functions.Mensagem("Erro: Não foi possível gerar o contrato aditivo. Por favor, confirme se não estão sendo fornecidas informações conflitantes na cotação.");
                }
            }
        };
        req.send(JSON.stringify(parameters));
    },

    Mensagem: function (msg) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: msg };
        var alertOptions = { height: 150, width: 280 };
        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {

            },
            function (error) {

            }
        );
    }
}
if (typeof (InvoiceJS) === undefined || typeof (InvoiceJS) === "undefined") { InvoiceJS = {}; }
/*
Utilizar comando sempre para cada nova função (trigger)
var smartHelper = new SmartHelper(_executionContext);
*/
InvoiceJS.Functions = {
    /*Evento default registrado no Onload do formulário*/
    OnLoad: function (_executionContext) {
        "use strict";
        var smartHelper = new SmartHelper(_executionContext);
        InvoiceJS.Functions.CustomFunction(smartHelper);
    },

    InvoiceOnchange: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        var status = formContext.getAttribute("statecode").getValue();
        var statusFat = formContext.getAttribute("msdyn_projectinvoicestatus").getValue();
        var numNota = formContext.getAttribute("smt_st_invoice_number").getValue();
        var dtEmissao = formContext.getAttribute("smt_dt_issue_invoice").getValue();
        var dtVenc = formContext.getAttribute("smt_dt_expiration_invoice").getValue();
        var dtRecebimento = formContext.getAttribute("smt_dt_receipt_invoice").getValue();


        if (status === 0) {

            if (numNota !== null) {

                //Habilita o preenchimento das datas de emissão e vencimento quando Nº estiver preenchido
                formContext.getControl("smt_dt_issue_invoice").setDisabled(false);
                formContext.getControl("smt_dt_expiration_invoice").setDisabled(false);
                //formContext.getAttribute("smt_dt_issue_invoice").setRequiredLevel("required");
                //formContext.getAttribute("smt_dt_expiration_invoice").setRequiredLevel("required");


                if (dtEmissao !== null) {
                    //Define status da fatura como 'Emitida'
                    formContext.getAttribute("msdyn_projectinvoicestatus").setValue(100000000);
                    formContext.getControl("smt_dt_receipt_invoice").setDisabled(false);

                } else {
                    //Retorna status para Rascunho caso o conteúdo campo dtEmissao seja apagado
                    formContext.getAttribute("msdyn_projectinvoicestatus").setValue(192350000)
                    formContext.getControl("smt_dt_receipt_invoice").setDisabled(true);
                    formContext.getAttribute("smt_dt_receipt_invoice").setValue(null);
                }

                if (dtRecebimento !== null && dtEmissao !== null) {
                    //Define status da fatura como 'Fatura Paga' caso o campo dtRecebimento seja preenchido
                    formContext.getAttribute("msdyn_projectinvoicestatus").setValue(192350001)

                } else if (dtRecebimento === null && dtEmissao !== null && statusFat !== 100000000) {
                    //Retorna status para 'Emitida' caso o conteúdo do campo dtRecebimento seja apagado e dtEmissao contenha dados.
                    formContext.getAttribute("msdyn_projectinvoicestatus").setValue(100000000)
                }


            } else {
                //Retorna campos para o estado original caso o conteúdo do campo Nº da Nota Fiscal seja apagado
                formContext.getAttribute("msdyn_projectinvoicestatus").setValue(192350000)
                formContext.getAttribute("smt_dt_issue_invoice").setValue(null);
                formContext.getControl("smt_dt_issue_invoice").setDisabled(true);
                formContext.getAttribute("smt_dt_receipt_invoice").setValue(null);
                formContext.getControl("smt_dt_receipt_invoice").setDisabled(true);
                formContext.getAttribute("smt_dt_expiration_invoice").setValue(null);
                formContext.getControl("smt_dt_expiration_invoice").setDisabled(true);
            }
        }
    },
};

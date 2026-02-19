if (typeof (Quote) === undefined || typeof (Quote) === "undefined") { Quote = {}; }

var Quote_LogicalName = "quote";
Quote.Functions = {
    VisibilityCloseAsWon: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();

        var additive = formContext.getAttribute("smt_pl_additive_contract");

        if (additive !== null) { //Se o campo existir no formulário

            if (additive.getValue() === 100000001) { //se o valor for "Não"
                return true;
            } else {
                return false;
            }
        }

        return true;
    },

    VisibilityAdditiveGenerate: function (primaryControl) {
        "use strict";
        //debugger;
        var additive = primaryControl.getAttribute("smt_pl_additive_contract");

        if (additive === null) return false;

        if (additive.getValue() !== 100000001 && additive.getValue() !== null) { //Se o valor for "Sim"
            var statecode = primaryControl.getAttribute("statecode").getValue();

            if (statecode === 1 || statecode === 0) return true; //Se statecode igual a "Ativo" ou "Rascunho"
        }

        return false;
    }

    //arrumar
    //regraBotaoGerarAditivo: function(formContext) {

    //var additive = formContext.getAttribute("smt_pl_additive_contract");
    //var _status = formContext.getAttribute("statecode");

    //if (additive.getValue() != null && _status.getValue() != null) { //Se o campo existir no formulário

    //    if (additive.getValue() === 100000000 && _status.getValue() === 0) { //se o valor for "SIM"
    //        return true;
    //    } else {
    //        return false;
    //    }
    //}

    //return false;
    //}
}

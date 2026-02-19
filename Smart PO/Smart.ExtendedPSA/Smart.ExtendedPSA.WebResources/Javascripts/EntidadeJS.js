if (typeof (EntidadeJS) === undefined || typeof (EntidadeJS) === "undefined") { EntidadeJS = {}; }
/*
Utilizar comando sempre para cada nova função (trigger)
var smartHelper = new SmartHelper(_executionContext);
*/
EntidadeJS.Functions = {
    /*Evento default registrado no Onload do formulário*/
    OnLoad: function (_executionContext) {
        var smartHelper = new SmartHelper(_executionContext);
        EntidadeJS.Functions.CustomFunction(smartHelper);
    },
    
    CustomFunction: function (smartHelper) {
        
    },
};
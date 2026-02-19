// Nome da entidade: Ajustes de Banco de Horas
if (typeof (TimeBankSettings) === undefined || typeof (TimeBankSettings) === "undefined") { TimeBankSettings = {}; }

var TimeBankSettings_LogicalName = "smt_timebank_settings";

TimeBankSettings.Functions = {
    //Evento default registrado no Onload do formulário
    OnLoad: function (executionContext) {
        "use strict";
        var smartHelper = new SmartHelper(executionContext);
    },

    AdjustmentType: function (executionContext, picklistField, hoursField) {
        "use strict";
        var smartHelper = new SmartHelper(executionContext);

        var picklistFieldValue = smartHelper._attributes.GetValue(picklistField);
        var hoursFieldValue = smartHelper._attributes.GetValue(hoursField);

        if (picklistFieldValue === 100000001) { // Pagamento de Horas
            //Se o usuário selecionar a opção "Pagamento de Horas", o sistema deverá deixar o campo "Horas" como negativas.
            if (hoursFieldValue > 0) {
                smartHelper._attributes.SetValue(hoursField, hoursFieldValue * -1);
            } else {
                smartHelper._attributes.ClearNotification(hoursField, null);
            }
        } else if (picklistFieldValue === 100000000) { // Saldo Inicial
            // Se o usuário selecionar a opção "Saldo Inicial", o sistema não deve deixar o usuário inserir horas negativas no campo "Horas".
            if (hoursFieldValue < 0) {
                smartHelper._attributes.AddNotification(hoursField, "ERROR", "Horas negativas não podem ser inseridas.");
            } else {
                smartHelper._attributes.ClearNotification(hoursField, null);
            }
        } else if (picklistFieldValue === 100000002) { // Ajuste Manual
            // Se o usuário selecionar a opção "Ajuste Manual", o sistema deve deixar o usuário colocar saldo negativo ou positivo.
            smartHelper._attributes.ClearNotification(hoursField, null);
        }
    },

    RequireNotes: function (executionContext, picklistField, notesField) {
        "use strict";
        var smartHelper = new SmartHelper(executionContext);

        var picklistFieldValue = smartHelper._attributes.GetValue(picklistField);

        if (picklistFieldValue === 100000001) { // Pagamento de Horas
            smartHelper._attributes.SetRequiredLevel(notesField, "none");
        } else if (picklistFieldValue === 100000000) { // Saldo Inicial
            smartHelper._attributes.SetRequiredLevel(notesField, "none");
        } else if (picklistFieldValue === 100000002) { // Ajuste Manual
            smartHelper._attributes.SetRequiredLevel(notesField, "required");
        }
    }
};
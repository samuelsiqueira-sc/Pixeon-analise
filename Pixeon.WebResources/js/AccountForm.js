if (typeof (Account) === undefined || typeof (Account) === "undefined") { Account = {}; }

var Account_LogicalName = "account";
Account.Functions = {
    formatDocument: function (executionContext) {
        debugger;
        var formContext = executionContext.getFormContext();

        var documentFormat = formContext.getAttribute("smt_st_document").getValue();
        if (documentFormat == null || documentFormat == "undefined") return;

        var document = documentFormat.replace(".", "").replace("/", "").replace("-", "");

        if (document.length == 11) {
            var _cpf = document;
            var cpf = new Object();
            cpf.success = false;
            cpf.message = "";
            cpf.formatedValue = "";
            var Soma;
            var Resto;
            Soma = 0;
            if (_cpf == "00000000000"
                || _cpf == "11111111111"
                || _cpf == "22222222222"
                || _cpf == "33333333333"
                || _cpf == "44444444444"
                || _cpf == "55555555555"
                || _cpf == "66666666666"
                || _cpf == "77777777777"
                || _cpf == "88888888888"
                || _cpf == "99999999999") {
                cpf.success = false;
                cpf.message = "CPF Inválido";
                formContext.getControl("smt_st_document").setNotification(cpf.message);
            }
            else {
                formContext.getControl("smt_st_document").clearNotification();
                for (var i = 1; i <= 9; i++) Soma = Soma + parseInt(_cpf.substring(i - 1, i)) * (11 - i);
                Resto = (Soma * 10) % 11;

                if ((Resto == 10) || (Resto == 11)) Resto = 0;
                if (Resto != parseInt(_cpf.substring(9, 10))) {
                    cpf.success = false;
                    cpf.message = "CPF Inválido";
                    formContext.getControl("smt_st_document").setNotification(cpf.message);
                }

                Soma = 0;
                for (var i = 1; i <= 10; i++) Soma = Soma + parseInt(_cpf.substring(i - 1, i)) * (12 - i);
                Resto = (Soma * 10) % 11;

                if ((Resto == 10) || (Resto == 11)) Resto = 0;
                if (Resto != parseInt(_cpf.substring(10, 11))) {
                    cpf.success = false;
                    cpf.message = "CPF Inválido";
                    formContext.getControl("smt_st_document").setNotification(cpf.message);
                }

                cpf.success = true;
                cpf.formatedValue = _cpf.substring(0, 3) + "." + _cpf.substring(3, 6) + "." + _cpf.substring(6, 9) + "-" + _cpf.substring(9, 11);
                formContext.getAttribute("smt_st_document").setValue(cpf.formatedValue);
            }
        }
        else if (document.length == 14) {
            var _cnpj = document;
            var cnpj = new Object();
            cnpj.success = false;
            cnpj.message = "";
            cnpj.formatedValue = "";

            if (_cnpj == "00000000000000" ||
                _cnpj == "11111111111111" ||
                _cnpj == "22222222222222" ||
                _cnpj == "33333333333333" ||
                _cnpj == "44444444444444" ||
                _cnpj == "55555555555555" ||
                _cnpj == "66666666666666" ||
                _cnpj == "77777777777777" ||
                _cnpj == "88888888888888" ||
                _cnpj == "99999999999999") {
                cnpj.success = false;
                cnpj.message = "CNPJ Inválido";
                formContext.getControl("smt_st_document").setNotification(cnpj.message);
            }
            else {
                formContext.getControl("smt_st_document").clearNotification();
                // Valida DVs
                var tamanho = _cnpj.length - 2
                var numeros = _cnpj.substring(0, tamanho);
                var digitos = _cnpj.substring(tamanho);
                var soma = 0;
                var pos = tamanho - 7;
                for (var i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0)) {
                    cnpj.success = false;
                    cnpj.message = "CNPJ Inválido";
                    formContext.getControl("smt_st_document").setNotification(cnpj.message);
                }

                tamanho = tamanho + 1;
                numeros = _cnpj.substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (var i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1)) {
                    cnpj.success = false;
                    cnpj.message = "CNPJ Inválido";
                    formContext.getControl("smt_st_document").setNotification(cnpj.message);
                }

                cnpj.success = true;
                cnpj.formatedValue = _cnpj.substring(0, 2) + "." + _cnpj.substring(2, 5) + "." + _cnpj.substring(5, 8) + "/" + _cnpj.substring(8, 12) + "-" + _cnpj.substring(12, 14);
                formContext.getAttribute("smt_st_document").setValue(cnpj.formatedValue);
            }
        }
    },
};
if (typeof (ProductExpiration) === undefined || typeof (ProductExpiration) === "undefined") { ProductExpiration = {}; }

"use strict";
ProductExpiration.Functions = {
    
    AlertaProdutoVencido: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        var globalContext = Xrm.Utility.getGlobalContext();
        var createdon;
        var smt_expiration;
        var smt_name;
        var result = null;
        var req = new XMLHttpRequest();

        //verifica a entidade "Smart Licenças" e pega a validade do produto

        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/smt_licenses?$select=createdon,smt_expiration,smt_name&$filter=smt_name eq 'PSA%20Extended'", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        createdon = results.value[i]["createdon"];
                        smt_expiration = results.value[i]["smt_expiration"];
                        smt_name = results.value[i]["smt_name"];

                        if (smt_expiration !== null) {
                            //separa a data do campo validade em dia, mes e ano

                            var now = new Date();
                            var dia = smt_expiration.substring(8, 10);
                            var mes = smt_expiration.substring(5, 7);
                            var ano = smt_expiration.substring(0, 4);
                            var vencimento = new Date(ano, mes - 1, dia, 0, 0, 0, 0);

                            if (vencimento <= now) {
                                var IdPSA = "22282531281715";
                                var message = "Produto de PSA vencido em: " + dia + "/" + mes + "/" + ano + "." + " Algumas funcionalidades podem estar indiponíveis. Para mais informações contate o fornecedor pelo e-mail: comercial@smartconsulting.com.br";
                                formContext.ui.setFormNotification(message, "WARNING", IdPSA);

                            }
                        } else {
                            var IdPSA = "22282531281715";
                            var message = "Não foi possível validar sua licença. Para mais informações contate o fornecedor pelo e-mail: comercial@smartconsulting.com.br";
                            formContext.ui.setFormNotification(message, "WARNING", IdPSA);
                        }

                    }
                } else {
                    ProductExpiration.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();


    },

    AlertaProdutoVencidoDemanda: function (executionContext) {
        "use strict";
        var formContext = executionContext.getFormContext();
        var globalContext = Xrm.Utility.getGlobalContext();
        var createdon;
        var smt_expiration;
        var smt_name;
        var result = null;
        var req = new XMLHttpRequest();

        //verifica a entidade "Smart Licenças" e pega a validade do produto

        req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/smt_licenses?$select=createdon,smt_expiration,smt_name&$filter=smt_name eq 'Smart%20Demandas'", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        createdon = results.value[i]["createdon"];
                        smt_expiration = results.value[i]["smt_expiration"];
                        smt_name = results.value[i]["smt_name"];

                        if (smt_expiration !== null) {
                            //separa a data do campo validade em dia, mes e ano

                            var now = new Date();
                            var dia = smt_expiration.substring(8, 10);
                            var mes = smt_expiration.substring(5, 7);
                            var ano = smt_expiration.substring(0, 4);
                            var vencimento = new Date(ano, mes - 1, dia, 0, 0, 0, 0);

                            if (vencimento <= now) {
                                var IdPSA = "22282531281715";
                                var message = "Produto de Demandas vencido em: " + dia + "/" + mes + "/" + ano + "." + " Algumas funcionalidades podem estar indiponíveis. Para mais informações contate o fornecedor pelo e-mail: comercial@smartconsulting.com.br";
                                formContext.ui.setFormNotification(message, "WARNING", IdPSA);

                            }
                        } else {
                            var IdPSA = "22282531281715";
                            var message = "Não foi possível validar sua licença. Para mais informações contate o fornecedor pelo e-mail: comercial@smartconsulting.com.br";
                            formContext.ui.setFormNotification(message, "WARNING", IdPSA);
                        }

                    }
                } else {
                    ProductExpiration.Functions.showAlertDialog(this.statusText);
                }
            }
        };
        req.send();


    },

    showAlertDialog: function (message) {
        "use strict";
        var message = { confirmButtonLabel: "OK", text: message };
        var alertOptions = { height: 150, width: 280 };

        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
            function success(result) {

            },
            function (error) {

            })
    }

}

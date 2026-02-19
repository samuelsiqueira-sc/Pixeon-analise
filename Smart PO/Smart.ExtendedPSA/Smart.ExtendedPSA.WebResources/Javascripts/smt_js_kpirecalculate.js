if (typeof (KPI) === undefined || typeof (KPI) === "undefined") { KPI = {}; }

var KPI_LogicalName = "msdyn_project";
KPI.Functions = {


    kpiRecalculated: function (primaryControl) // Função responsável por pegar o id do projeto, criar messsage box e chamar request
    {
        "use strict";
        var ProjectId; //Pega o Id do projeto atual
        var Request;  // Faz o request em atributos do recurso


        ProjectId = primaryControl.data.entity.getId().replace("{", "").replace("}", ""); // Deixa o Id sem "{" e "}"
        Request = KPI.Functions.requestAtribuiçãoRecurso(ProjectId); // Chama o request com os id's obtidos

    },



    executeRequest: function (Request) {
        "use strict";
        if (Request.value.length !== null) // Verifica se foi trazido algum resultado no request
        {
            KPI.Functions.setValueToday(Request);

        }

    },

    requestAtribuiçãoRecurso: function (ProjectId) // Request chama a entidade atribuições do recurso
    {
        "use strict";

        var globalContext = Xrm.Utility.getGlobalContext();

        Xrm.WebApi.online.retrieveMultipleRecords("msdyn_resourceassignment", "?$filter=_msdyn_projectid_value eq " + ProjectId).then(
            function success(results) {
                for (var i = 0; i < results.entities.length; i++) {
                    var msdyn_resourceassignmentid = results.entities[i]["msdyn_resourceassignmentid"];
                }
                // Cria uma message box perguntando se o usuário gostaria de recalcular
                var confirmStrings = { text: "Gostaria de recalcular o custo total planejado ?", title: "Recalculo de KPI." };
                var confirmOptions = { height: 200, width: 450 };
                Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                    function (success) {
                        if (success.confirmed) // Caso positivo será chamado a função com os requests
                        {
                            KPI.Functions.executeRequest(results);
                        }
                        else {
                        }
                    });
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
        //var req = new XMLHttpRequest();
        //req.open("GET", globalContext.getClientUrl() + "/api/data/v9.1/msdyn_resourceassignments?$select=msdyn_resourceassignmentid&$filter=_msdyn_projectid_value eq " + ProjectId + "", false);
        //req.setRequestHeader("OData-MaxVersion", "4.0");
        //req.setRequestHeader("OData-Version", "4.0");
        //req.setRequestHeader("Accept", "application/json");
        //req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        //req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        //req.onreadystatechange = function () {
        //    if (this.readyState === 4) {
        //        req.onreadystatechange = null;
        //        if (this.status === 200) {
        //            results = JSON.parse(this.response);
        //            for (var i = 0; i < results.value.length; i++) {
        //                var msdyn_resourceassignmentid = results.value[i]["msdyn_resourceassignmentid"];
        //            }
        //        } else {
        //            KPI.Functions.showAlertDialog(this.statusText);
        //        }
        //    }
        //};
        //req.send();

        //return results;
    },

    setValueToday: function (Request) // Request atualiza a entidade atribuições do recurso, no campo ultima atualização
    {
        "use strict";
        var now;
        var idReq;
        now = new Date(); //  Pega a data atual

        for (var i = 0; i < Request.value.length; i++) // Varre o vetor do request, pega os id e cria um update
        {
            var entity = {};
            entity.smt_ultima_atualizacao_pp = now;

            idReq = Request.value[i]['msdyn_resourceassignmentid'];

            Xrm.WebApi.online.updateRecord("msdyn_resourceassignment", "" + idReq + "", entity).then(
                function success(result) {
                    var updatedEntityId = result.idReq;
                },
                function (error) {
                    KPI.Functions.showAlertDialog(error.message);
                }
            );

            if (i >= 39) {
                for (var j = i; j < Request.value.length; j++) // Varre o vetor do request, pega os id e cria um update
                {
                    var entity = {};
                    entity.smt_ultima_atualizacao_pp = now;

                    idReq = Request.value[j]['msdyn_resourceassignmentid'];

                    Xrm.WebApi.online.updateRecord("msdyn_resourceassignment", "" + idReq + "", entity).then(
                        function success(result) {
                            var updatedEntityId = result.idReq;
                        },
                        function (error) {
                            KPI.Functions.showAlertDialog(error.message);
                        }
                    );
                }
            }
        }
    }
}
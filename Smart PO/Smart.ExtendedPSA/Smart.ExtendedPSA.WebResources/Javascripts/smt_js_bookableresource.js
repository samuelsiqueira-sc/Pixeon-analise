if (typeof (bookableresource) === undefined || typeof (bookableresource) === "undefined") { bookableresource = {}; }

var bookableresource_LogicalName = "bookableresource";
bookableresource.Functions = {

    // Constante com o nome do Parâmetro Smart.
    ParametersName: {
        RHParameterHoliday: "GET TEAM RH FOR HOLIDAY REQUEST",
    },

    callFlow: async function (primaryControl) {
        "use strict";
        //debugger;
        // Obtêm o ID da unidade organizacional do recurso reservável.
        var formcontext = primaryControl.getFormContext();
        var organizationalUnit = formcontext.getAttribute("msdyn_organizationalunit").getValue();
        if (organizationalUnit !== null && organizationalUnit !== undefined) {
            organizationalUnit = organizationalUnit[0].id.replace("{", "").replace("}", "");
        }

        // Retrieve na entidade "Parâmetro Smart" onde o campo unidade organizacional seja igual a BU do recurso e o nome do parâmetro seja "GET TEAM RH FOR HOLIDAY REQUEST"
        var valueParameter = await ExtendedHelp.Functions.retrieveParameterSmart(organizationalUnit, this.ParametersName.RHParameterHoliday);

        // Retrieve na entidade "Equipe" onde o "Nome da Equipe" seja igual ao campo "Valor do parâmetro."
        var team = await ExtendedHelp.Functions.retrieveTeam(valueParameter);
        if (team !== undefined && team !== null) {

            // Obtêm o ID do usuário logado no sistema.
            var userSettings = Xrm.Utility.getGlobalContext().userSettings;
            var user = userSettings.userId;
            if (user !== null && user !== undefined) {
                user = user.replace("{", "").replace("}", "");
            }
            // Retrieve que verifica se o usuário logado no sistema é membro da equipe que foi obtida na função "retrieveTeam."
            var isTeam = await ExtendedHelp.Functions.retrieveTeamMemberShip(user, team);

            // Obtêm o ID da entidade "Recurso Reservável, para posteriormente chamar o flow, quando houver alteraçôes na entidade"
            var bookableresourceid = formcontext.data.entity.getId("bookableresourceid");
            if (bookableresourceid !== null && bookableresourceid !== undefined) {
                bookableresourceid = bookableresourceid.replace("{", "").replace("}", "");

            }
            // Verifica qual o tipo do formulário.
            var type = formcontext.ui.getFormType();

            // Se o formulário for do tipo "Atualização" e o usuário logado não pertencer a equipe "RH GLOBAL" o flow deve ser chamado.
            if (type === 2 && !isTeam) {
                var settings = {
                    "async": true,
                    "crossDomain": true,
                    "url": "https://prod-15.brazilsouth.logic.azure.com:443/workflows/b5c9ebb707574e1a90674055d599743a/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=QK7z4c4Zd4KFGiStoJwyZV1yt0VRohASd0sYUA7fypE",
                    "method": "POST",
                    "headers": {
                        "Content-Type": "application/json",
                        "User-Agent": "PostmanRuntime/7.19.0",
                        "Accept": "*/*",
                        "Cache-Control": "no-cache",
                        "Postman-Token": "2d02d71b-358f-49cf-94bb-c62d9f6adcb7,f254029b-e0d7-405f-8fad-209a2779c59a",
                        "Host": "prod-15.brazilsouth.logic.azure.com:443",
                        "Accept-Encoding": "gzip, deflate",
                        "Content-Length": "0",
                        "Connection": "keep-alive",
                        "cache-control": "no-cache"
                    },
                    "processData": false,
                    "data": "{\r\n \"bookableresourceid\": \"" + bookableresourceid + "\",\n}"
                }

                $.ajax(settings).done(function (response) {
                    // console.log(response);
                });
            }

        }
    }
}
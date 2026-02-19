if (typeof (ExtendedHelp) == "undefined") { ExtendedHelp = {}; }
/*
Utilizar comando sempre para cada nova função (trigger)
var smartHelper = new SmartHelper(_executionContext);
*/
ExtendedHelp.Functions = {
    /*Evento default registrado no Onload do formulário*/

    ParametersName: {
        ManagerParameterHoliday: "GET TEAM PROJECT MANAGER FOR HOLIDAY REQUEST",
        RHParameterHoliday: "GET TEAM RH FOR HOLIDAY REQUEST"
    },

    retrieveUnitOrganization(id) {
        debugger;
        id = id.replace("{", "").replace("}", "");
        //retrive single no recurso e buscar a unidade organizacional (retornar)
        var result = null;
        //Use XMLHTTP
        var req = new XMLHttpRequest();
		req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/bookableresources(" + id + ")?$select=_msdyn_organizationalunit_value", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    result = JSON.parse(this.response);
                    var _msdyn_organizationalunit_value = result["_msdyn_organizationalunit_value"];
                    var _msdyn_organizationalunit_value_formatted = result["_msdyn_organizationalunit_value@OData.Community.Display.V1.FormattedValue"];
                    var _msdyn_organizationalunit_value_lookuplogicalname = result["_msdyn_organizationalunit_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
        return result;
    },

    retrieveParameterSmart(idUO, ManagerParameterHoliday) {
        debugger;

        idUO = idUO.replace("{", "").replace("}", "");

        var results = null;
        var smt_value = null;
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/smt_smartparameters?$select=smt_name,smt_value&$filter=_smt_organizationalunit_value eq " + idUO + " and  smt_name eq '" + ManagerParameterHoliday + "'", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    results = JSON.parse(this.response);
                    if (results.value.length > 0) {
                        for (var i = 0; i < results.value.length; i++) {
                            var smt_name = results.value[i]["smt_name"];
                            smt_value = results.value[i]["smt_value"];
                        }
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        if (results == undefined) {

            var req = new XMLHttpRequest();
			req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/smt_smartparameters?$filter=smt_name eq '" + ManagerParameterHoliday + "' and  smt_smartparameterid eq null", false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        results = JSON.parse(this.response);
                        for (var i = 0; i < results.value.length; i++) {
                            var smt_name = results.value[i]["smt_name"];
                            var smt_value = results.value[i]["smt_value"];
                        }
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            };
            req.send();
        }
        return smt_value;
    },

    retrieveTeam(valueParameter) {
        debugger;

        var teamid = null;
		var req = new XMLHttpRequest();
		req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/teams?$select=teamid&$filter=name eq '" + valueParameter + "'", false);
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
                        teamid = results.value[i]["teamid"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();

        return teamid;
    },

    retrieveTeamMemberShip(idResource, teamId) {
        debugger;

        Xrm.WebApi.online.retrieveMultipleRecords("teammembership", "?$filter=systemuserid eq " + idResource + " and  teamid eq " + teamId + "").then(
            function success(results) {
                for (var i = 0; i < results.entities.length; i++) {
                    var teammembershipid = results.entities[i]["teammembershipid"];
                }
                if (results.entities.length > 0) {
                    return true;
                }
                else {
                    return false;
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
    },

	updateStateCode(id) {
		debugger;
		var entity = {};
		entity.statecode = 1;

		Xrm.WebApi.online.updateRecord("smt_holiday_request", ""+ id +"", entity).then(
			function success(result) {
				var updatedEntityId = result.id;
			},
			function (error) {
				Xrm.Utility.alertDialog(error.message);
			}
		);

	}

};
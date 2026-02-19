if (typeof (ExtendedHelp) === undefined || typeof (ExtendedHelp) === "undefined") { ExtendedHelp = {}; }
/*
Utilizar comando sempre para cada nova função (trigger)
var smartHelper = new SmartHelper(_executionContext);
*/
"use strict";
ExtendedHelp.Functions = {
    /*Evento default registrado no Onload do formulário*/

    ParametersName: {
        ManagerParameterHoliday: "GET TEAM PROJECT MANAGER FOR HOLIDAY REQUEST",
        RHParameterHoliday: "GET TEAM RH FOR HOLIDAY REQUEST"
    },

    RolesName: {
        ProjectManagerExtended: "Extended Project Manager",
        ProjectManager: "Project Manager"
    },

    retrieveUserId: async function(idResource) {
        "use strict";
        idResource = idResource.replace("{", "").replace("}", "");
        var result = await Xrm.WebApi.retrieveRecord("bookableresource", idResource, "?$select=_userid_value").then(
            function success(result) {
                // console.log(result);
                // Columns
                var bookableresourceid = result["bookableresourceid"]; // Guid
                var userid = result["_userid_value"]; // Lookup
                var userid_formatted = result["_userid_value@OData.Community.Display.V1.FormattedValue"];
                var userid_lookuplogicalname = result["_userid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                return result;
            },
            function (error) {
                return null;
            }
        );

        return result;
    },


    retrieveUnitOrganization: async function (id) {
        "use strict";
        // debugger;
        id = id.replace("{", "").replace("}", "");
        //retrive single no recurso e buscar a unidade organizacional (retornar)
        
        // debugger;
        
        var results = await Xrm.WebApi.retrieveRecord("bookableresource", id , "?$select=_msdyn_organizationalunit_value").then(
            function success(result) {
                // debugger;
                return result;
                // Columns
                var bookableresourceid = result["bookableresourceid"]; // Guid
                var msdyn_organizationalunit = result["_msdyn_organizationalunit_value"]; // Lookup
                var msdyn_organizationalunit_formatted = result["_msdyn_organizationalunit_value@OData.Community.Display.V1.FormattedValue"];
                var msdyn_organizationalunit_lookuplogicalname = result["_msdyn_organizationalunit_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
            },
            function (error) {
                return null;
            }
        );
        
        return results;
    },

    retrieveParameterSmart: async function(idUO, ManagerParameterHoliday) {
        "use strict";
        idUO = idUO.replace("{", "").replace("}", "");
        var smt_value = null;
        // Busca o parâmetro da unidade
        var results = await Xrm.WebApi.retrieveMultipleRecords("smt_smartparameter", "?$filter=(_smt_organizationalunit_value eq " + idUO + " and smt_name eq '"+ ManagerParameterHoliday +"')").then(
            function success(results) {
                
                return results;
               
            },
            function (error) {
                return null; //console.log(error.message);
            }
        );
        // debugger;
        // Se tiver vazio Busca o parâmetro gobal
        if (results === null || results === undefined || results.entities.length === 0) {
            
            // Busca o Parâmetro Global
            results = null;
            results = await Xrm.WebApi.retrieveMultipleRecords("smt_smartparameter", "?$filter=(_smt_organizationalunit_value eq null and smt_name eq '"+ ManagerParameterHoliday +"')").then(
                function success(results) {
                    
                    return results;
                    
                },
                function (error) {
                    Xrm.Navigation.openAlertDialog(error.message)
                }
            );
            // debugger;

            smt_value = results.entities[0]["smt_value"];

        } else {
            smt_value = results.entities[0]["smt_value"];
        }
        return smt_value;
    },

    retrieveTeam: async function (valueParameter) {
        "use strict";
        var teamid = null;
        var teamid = null;
        
        var result = await Xrm.WebApi.retrieveMultipleRecords("team", "?$filter=name eq '"+ valueParameter +"'").then(
            function success(results) {
                
                return results;
                
            },
            function (error) {
                // console.log(error.message);
            }
        );
       
        teamid = result !== null ? teamid = result.entities[0]["teamid"] : null;

        return teamid;
    },

    retrieveTeamMemberShip: async function (userId, teamId) {
        "use strict";
        userId = userId.replace("{", "").replace("}", "");
        
        var result = await Xrm.WebApi.retrieveMultipleRecords("teammembership", "?$filter=(systemuserid eq "+ userId +" and teamid eq "+ teamId +")").then(
            function success(results) {

                return results;                
            },
            function (error) {
                return null;
            }
        );
        
        if (result === null || result === undefined || result.entities.length === 0) {

            return false;
        }
        else {
            return true;
        }
    },


    updateStatusCode(guid, status) {
        "use strict";

        guid = guid.replace("{", "").replace("}", "");
        var parameters = {};
        var entity = {};
        entity.id = guid;
        entity.entityType = "smt_holiday_request";
        parameters.entity = entity;

        var smt_AC_UpdateStatusCode = {
            entity: parameters.entity,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.smt_holiday_request",
                            "structuralProperty": 5
                        }
                    },
                    operationType: 0,
                    operationName: "smt_AC_UpdateStatusCode"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_UpdateStatusCode).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                    var entityFormOptions = {};
                    entityFormOptions["entityName"] = "smt_holiday_request";
                    entityFormOptions["entityId"] = guid;
                    Xrm.Navigation.openForm(entityFormOptions, guid);
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    updateStatusCodeApprove(guid, status) {
        "use strict";
        guid = guid.replace("{", "").replace("}", "");
        var parameters = {};
        var entity = {};
        entity.id = guid;
        entity.entityType = "smt_holiday_request";
        parameters.entity = entity;

        var smt_AC_ApproveVacationRequest = {
            entity: parameters.entity,

            getMetadata: function () {
                return {
                    boundParameter: "entity",
                    parameterTypes: {
                        "entity": {
                            "typeName": "mscrm.smt_holiday_request",
                            "structuralProperty": 5
                        },
                    },
                    operationType: 0,
                    operationName: "smt_AC_ApproveVacationRequest"
                };
            }
        };

        Xrm.WebApi.online.execute(smt_AC_ApproveVacationRequest).then(
            function success(result) {
                if (result.ok) {
                    //Success - No Return Data - Do Something
                    var entityFormOptions = {};
                    entityFormOptions["entityName"] = "smt_holiday_request";
                    entityFormOptions["entityId"] = guid;
                    Xrm.Navigation.openForm(entityFormOptions, guid);
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog(error.message);
            }
        );
    },

    request: async function (id) {
        "use strict";
        var originalFetchXML = `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
	                            <entity name="role">
		                            <attribute name="name" />
		                            <attribute name="businessunitid" />
		                            <attribute name="roleid" />
		                            <order attribute="name" descending="false" />
		                            <link-entity name="systemuserroles" from="roleid" to="roleid" visible="false" intersect="true">
			                            <link-entity name="systemuser" from="systemuserid" to="systemuserid" alias="ab">
				                            <filter type="and">
					                            <condition attribute="systemuserid" operator="eq" value="`+ id + `" />
				                            </filter>
			                            </link-entity>
		                            </link-entity>
	                            </entity>
                            </fetch>`;
        var escapedFetchXML = encodeURIComponent(originalFetchXML);

        var result = await Xrm.WebApi.retrieveMultipleRecords("smt_holiday_request", "?fetchXml=" + escapedFetchXML).then(
            function success(results) {
                return results;
            },
            function (error) {
                return null;
            }
        );
        
        /*
        results = null;
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/roles?fetchXml=%3Cfetch%20version%3D%221.0%22%20output-format%3D%22xml-platform%22%20mapping%3D%22logical%22%20distinct%3D%22true%22%3E%3Centity%20name%3D%22role%22%3E%3Cattribute%20name%3D%22name%22%20%2F%3E%3Cattribute%20name%3D%22businessunitid%22%20%2F%3E%3Cattribute%20name%3D%22roleid%22%20%2F%3E%3Corder%20attribute%3D%22name%22%20descending%3D%22false%22%20%2F%3E%3Clink-entity%20name%3D%22systemuserroles%22%20from%3D%22roleid%22%20to%3D%22roleid%22%20visible%3D%22false%22%20intersect%3D%22true%22%3E%3Clink-entity%20name%3D%22systemuser%22%20from%3D%22systemuserid%22%20to%3D%22systemuserid%22%20alias%3D%22ab%22%3E%3Cfilter%20type%3D%22and%22%3E%3Ccondition%20attribute%3D%22systemuserid%22%20operator%3D%22eq%22%20value%3D%22" + id + "%22%20%2F%3E%3C%2Ffilter%3E%3C%2Flink-entity%3E%3C%2Flink-entity%3E%3C%2Fentity%3E%3C%2Ffetch%3E", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    results = JSON.parse(this.response);
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        results = await req.send();
        */
        return results;
    },


    InativeHoliday: async function (primaryControl) {
        "use strict";
        var id = primaryControl.data.entity.getId();
        id = id.replace("{", "").replace("}", "");

        var record = {};
        record.statuscode = 100000001; // Status
        record.statecode = 1; // State

        await Xrm.WebApi.updateRecord("smt_holiday_request", id, record).then(
            function success(result) {
                var updatedId = result.id;
               // console.log(updatedId);
            },
            function (error) {
                // console.log(error.message);
            }
        );
        /*
        var entity = {};
        entity.statecode = 1;
        entity.statuscode = 100000001;

        var id = primaryControl.data.entity.getId();
        id = id.replace("{", "").replace("}", "");

        var req = new XMLHttpRequest();
        req.open("PATCH", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/smt_holiday_requests(" + id + ")", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success - No Return Data - Do Something
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        await req.send(JSON.stringify(entity));
        */
        
        primaryControl.data.refresh();
    }

};
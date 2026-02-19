if (typeof (CalculateRollup) === undefined || typeof (CalculateRollup) === "undefined") { CalculateRollup = {}; }
"use strict";
CalculateRollup.Functions = {

	calcule: function (primaryControl) {
		"use strict";
		// debugger;
		var formContext = primaryControl;
		var parameters = {};
		parameters.Entity = "bookableresource";
		parameters.Id = formContext.data.entity.getId().replace("{", "").replace("}", "");;
		parameters.Fields = "smt_dc_paidhours_balance;smt_dc_timebank_balance";

		var smt_AC_CalculateRollupRequest = {
			Entity: parameters.Entity,
			Id: parameters.Id,
			Fields: parameters.Fields,

			getMetadata: function () {
				return {
					boundParameter: null,
					parameterTypes: {
						"Entity": {
							"typeName": "Edm.String",
							"structuralProperty": 1
						},
						"Id": {
							"typeName": "Edm.String",
							"structuralProperty": 1
						},
						"Fields": {
							"typeName": "Edm.String",
							"structuralProperty": 1
						}
					},
					operationType: 0,
					operationName: "smt_AC_CalculateRollup"
				};
			}
		};

		Xrm.WebApi.online.execute(smt_AC_CalculateRollupRequest).then(
			function success(result) {
				if (result.ok) {
					//Success - No Return Data - Do Something
				}
			},
			function (error) {
				Xrm.Navigation.openAlertDialog(error.message);
			}
		);
		setTimeout(function () { CalculateRollup.Functions.refresh(formContext); }, 1000);
		// setTimeout(function () { CalculateRollup.Functions.refresh(); }, 1000);
	},

	refresh: function (formContext) {
		"use strict";
		formContext.data.refresh(true);
		//Xrm.Page.data.refresh();
	}
}
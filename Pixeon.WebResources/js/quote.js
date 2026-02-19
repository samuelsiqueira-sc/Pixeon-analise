if (typeof (quote) === undefined || typeof (quote) === "undefined") { quote = {}; }

var quote_LogicalName = "quote";
quote.Functions = {
	arquivar: function (primaryControl) {

		var id = primaryControl.data.entity.getId().replace("{", "").replace("}", "");

		var parameters = {};
		var entity = {};
		entity.id = id;
		entity.entityType = "quote";
		parameters.entity = entity;

		var smt_AC_fileQuoteRequest = {
			entity: parameters.entity,

			getMetadata: function () {
				return {
					boundParameter: "entity",
					parameterTypes: {
						"entity": {
							"typeName": "mscrm.quote",
							"structuralProperty": 5
						}
					},
					operationType: 0,
					operationName: "smt_AC_fileQuote"
				};
			}
		};

		Xrm.WebApi.online.execute(smt_AC_fileQuoteRequest).then(
			function success(result) {
				if (result.ok) {
					//Success - No Return Data - Do Something
					primaryControl.data.refresh();
				}
			},
			function (error) {
				Xrm.Utility.alertDialog(error.message);
			}
		);
	}
}
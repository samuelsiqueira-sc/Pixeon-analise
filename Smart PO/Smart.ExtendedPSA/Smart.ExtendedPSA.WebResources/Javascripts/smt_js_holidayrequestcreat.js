if (typeof (HolidayRequestCreat) === undefined || typeof (HolidayRequestCreat) === "undefined") { HolidayRequestCreat = {}; }

var HolidayRequestCreat_LogicalName = "smt_holiday_request";
HolidayRequestCreat.Functions = {

	ValueAction: function (executionContext) {
		"use strict";
		formContext = executionContext.getFormContext();

		var HolidayBegin = formContext.getAttribute("smt_dt_holiday_start_date").getValue();
		var HolidayEnd = formContext.getAttribute("smt_dt_holiday_end_date").getValue()
		var Id = formContext.getAttribute("smt_tx_register_id").getValue();

		var NewId = Id.split(',');


		for (var i = 0; i <= NewId.length; i++) {

			var entity = {};
			entity.smt_dt_end = new Date(HolidayEnd).toISOString();
			entity.smt_dt_start = new Date(HolidayBegin).toISOString();
			entity.smt_smt_pl_type = 100000001;
			entity.statuscode = 2;
			entity["smt_lp_resource@odata.bind"] = "/bookableresources(" + NewId[i] + ")";

			Xrm.WebApi.online.createRecord("smt_holiday_request", entity).then(
				function success(result) {
					var newEntityId = result.id;
				},
				function (error) {
					showAlertDialog(error.message);
				}
			);

		}

	},

	showAlertDialog: function (message) {
		"use strict";
		var message = { confirmButtonLabel: "OK", text: message };
		var alertOptions = { height: 150, width: 280 };

		Xrm.Navigation.openAlertDialog(message, alertOptions).then(
			function success(result) {
				//console.log("Alert dialog closed");
			},
			function (error) {
				//console.log(error.message);
			})
	}
}

var Sdk = window.Sdk || {};
(
    function () {
        "use strict";
        this.NavigateToJustify = function (executionContext) {
            var qs = "param1=1&param2=2";
            var pageInput = {
                pageType: "webresource",
                webresourceName: "https://smartpsadev.crm2.dynamics.com//WebResources/smt_html_form_justificativa",
                data: encodeURIComponent(qs)
            };
            var navigationOptions = {
                target: 2, // 2 is for opening the page as a dialog. 
                width: 400, // default is px. can be specified in % as well. 
                height: 300, // default is px. can be specified in % as well. 
                position: 1 // Specify 1 to open the dialog in center; 2 to open the dialog on the side. Default is 1 (center). 
            };
            Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
                function success() {
                    // debugger;
                },
                function error(e) {
                    // Handle errors 
                }
            );

        }
    }

).call(Sdk);
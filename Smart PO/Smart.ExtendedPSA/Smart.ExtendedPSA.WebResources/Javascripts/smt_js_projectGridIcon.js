//display icon and tooltio for the grid column  
function displayIconCPI(rowData, userLCID) {
    "use strict";
    var str = JSON.parse(rowData);
    var coldata = str.smt_pl_cost_performance_Value;
    var imgName = "";
    //var tooltip = "";
    switch (coldata) {
        case 0:
            imgName = "smt_/images/red.png";
            //tooltip = "Estouro no Orçamento";
            break;
        case 1:
            imgName = "smt_/images/green.png";
            //tooltip = "Dentro do Orçamento";
            break;
        case 2:
            imgName = "smt_/images/green.png";
            //tooltip = "Economizando";
            break;
        default:
            imgName = "";
            //tooltip = "";
            break;
    }
    var resultarray = [imgName];
    return resultarray;
}

//display icon and tooltio for the grid column  
function displayIconSPI(rowData, userLCID) {
    "use strict";
    var str = JSON.parse(rowData);
    var coldata = str.smt_pl_term_performance_Value;
    var imgName = "";
    //var tooltip = "";
    switch (coldata) {
        case 0:
            imgName = "smt_/images/red.png";
            //tooltip = "Atrasado";
            break;
        case 1:
            imgName = "smt_/images/green.png";
            //tooltip = "Dentro do Prazo";
            break;
        case 2:
            imgName = "smt_/images/green.png";
            //tooltip = "Adiantado";
            break;
        default:
            imgName = "";
            //tooltip = "";
            break;
    }
    var resultarray = [imgName];
    return resultarray;
}

//display icon and tooltio for the grid column  
function displayIconVarMargemContrato(rowData, userLCID) {
    "use strict";
    var str = JSON.parse(rowData);
    var coldata = str.smt_pl_percentagecontractmargin_Value;
    var imgName = "";
    //var tooltip = "";
    switch (coldata) {
        case 0:
            imgName = "smt_/images/red.png";
            //tooltip = "Projeto Fora da Margem";
            break;
        case 1:
            imgName = "smt_/images/green.png";
            //tooltip = "Projeto Dentro da Margem";
            break;
        default:
            imgName = "";
            //tooltip = "";
            break;
    }
    var resultarray = [imgName];
    return resultarray;
}

function displaytimeEntriesToApprove(rowData, userLCID) {
    "use strict";
    var str = JSON.parse(rowData);
    var hourTypeID = gridEntity.getId("smt_lp_type_hours");
    var field = ProjectView.Functions.retrievePl(hourTypeID);
    var coldata = field_Value;
    var imgName = "";
    //var tooltip = "";
    switch (coldata) {
        case 100000002:
            imgName = "smt_/images/red.png";
            //tooltip = "Horas Extras;
            break;
        case 100000000:
            imgName = "smt_/images/green.png";
            //tooltip = "Horas Normais";
            break;
        case 100000001:
            imgName = "smt_/images/yellow.png";
            //tooltip = "Banco de Horas";
            break;
        default:
            imgName = "";
            //tooltip = "";
            break;
    }
    var resultarray = [imgName];
    return resultarray;
}

function retrievePl(hourTypeID) {
    "use strict";
    var globalContext = Xrm.Utility.getGlobalContext();
    var req = new XMLHttpRequest();
    req.open("GET", globalContext.context.getClientUrl() + "/api/data/v9.1/smt_type_hourses(" + hourTypeID + ")?$select=smt_pl_type_hours", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                var smt_pl_type_hours = result["smt_pl_type_hours"];
                var smt_pl_type_hours_formatted = result["smt_pl_type_hours@OData.Community.Display.V1.FormattedValue"];
            } else {
                ProjectView.Functions.showAlertDialog(this.statusText);
            }
        }
    };
    req.send();
}

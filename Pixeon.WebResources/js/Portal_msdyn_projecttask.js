$(document).ready(function () {
	// Chamada da função.
	HideShowFields();
})
// Função que mostra o campo "Aceite do Cliente" se o campo "Data de Vencimento de Aprovação" conter dados e esconde o mesmo caso contrário.
function HideShowFields() {

	// Guarda o valor do campo "Data de Vencimento de Aprovação" na variável.
	var value = $("#smt_dt_expired_approval").val();

	if (value != 0) {
		showField("smt_pl_acceptance");
	}
	else {
		hideField("smt_pl_acceptance");
	}
}

// Função que mostra o campo no formulário.
function showField(fieldProvided) {
	$("#" + fieldProvided).closest('tr').show();
}

// Função que esconde o campo no formulário.
function hideField(fieldProvided) {
	$("#" + fieldProvided).closest('tr').hide();
}


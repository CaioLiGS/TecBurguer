$("#Cep").blur(function () {
    var cep = $(this).val().replace(/\D/g, '');

    if (cep.length === 8) {
        $.getJSON(`https://viacep.com.br/ws/${cep}/json/`, function (data) {
            if (!("erro" in data)) {
                $("#Rua").val(data.logradouro);
                $("#Bairro").val(data.bairro);
                $("#Cidade").val(data.localidade);
            } else {
                alert("CEP não encontrado.");

            }
        });
    }
});

$("#Localidade").blur(function(){
    var localidade = $(this).val();

    if(localidade == "Condomínio"){
        $("#NumeroCasaLabel").text("Nome do condomínio / Complemento")
    } else {
        $("#NumeroCasaLabel").text("Numero da casa")
    }
});

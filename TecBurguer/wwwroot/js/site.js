//$("#Cep").blur(function () {
//    var cep = $(this).val().replace(/\D/g, '');

//    if (cep.length === 8) {
//        $.getJSON(`https://viacep.com.br/ws/${cep}/json/`, function (data) {
//            if (!("erro" in data)) {
//                $("#Rua").val(data.logradouro);
//                $("#Bairro").val(data.bairro);
//                $("#Cidade").val(data.localidade);
//            } else {
//                alert("CEP não encontrado.");

//            }
//        });
//    }
//});

//$("#Localidade").blur(function(){
//    var localidade = $(this).val();

//    if(localidade == "Condomínio"){
//        $("#NumeroCasaLabel").text("Nome do condomínio / Complemento")
//    } else {
//        $("#NumeroCasaLabel").text("Numero da casa")
//    }
//});

function adicionarAoCarrinho(id, emailUsuario) {
    console.log(emailUsuario);
    // 1. Coleta os dados do hambúrguer
    const elementoNome = document.getElementById("NomeBurguer_" + id);
    const elementoPreco = document.getElementById("PrecoBurguer_" + id);

    if (!elementoNome) {
        console.error("Elemento 'NomeBurguer_' não encontrado.");
        return;
    }

    const nome = elementoNome.textContent;
    const preco = parseFloat(elementoPreco.textContent.replace('R$', ''));
    var user = 0;

    axios.get('/api/usuarios/listar').then(function (response) {

        console.log(response.data);

        for (var i = 0; i < response.data.length; i++) {
            if (emailUsuario == response.data[i].email) {
                user = response.data[i].idUsuario;
                console.log(user);
                break;
            }
        }
        
    });

    const dados = {
        nome: nome,
        PrecoTotal: preco,
        estado: "Decidindo",
        idUsuario: user,
        idHamburguer: id
    };


    const url = 'https://localhost:7192/api/pedidos/create';

    // 3. Usa o Axios para enviar a requisição POST
    axios.post(url, dados)
        .then(response => {
            console.log('Sucesso! Resposta do servidor:', response.data);
        })
        .catch(error => {
            console.error('Erro ao adicionar pedido:', error);
        });
}
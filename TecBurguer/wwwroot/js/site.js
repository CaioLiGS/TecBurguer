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

function adicionarPedidosHamburgueres(response, num) {
    var url = 'api/pedidohamburgueres/create';

    axios.post(url, { IdPedido: response.data[num].IdPedido, IdHamburguer: id })
        .then(x => {
            console.log('Sucesso! Resposta do servidor:', x.data);
        })
        .catch(error => {
            console.error('Erro ao adicionar pedido:', error);
        });
}

function posicaoUsuarioEPedidos(user, response) {
    for (var i = 0; i < response.data.length; i++) {
        if (user == response.data[i].IdUsuario) {
            return i
        }
    }
}

function adicionarPedidos(nome, preco, user) {
    const dados = {
        nome: nome,
        PrecoTotal: preco,
        estado: "Decidindo",
        IdUsuario: user
    };

    var url = 'api/pedidos/create';

    axios.post(url, dados)
        .then(response => {
            console.log('Criou pedidos', response.data);
        })
        .catch(error => {
            console.error('Erro ao adicionar pedido:', error);
        });
}

function adicionarAoCarrinho(id, emailUsuario) {
    console.log(emailUsuario);
    
    const elementoNome = document.getElementById("NomeBurguer_" + id);
    const elementoPreco = document.getElementById("PrecoBurguer_" + id);

    if (!elementoNome) {
        console.error("Elemento 'NomeBurguer_' não encontrado.");
        return;
    }

    const nome = elementoNome.textContent;
    const preco = parseFloat(elementoPreco.textContent.replace('R$', ''));

    // Pegando os usuários
    axios.get('/api/usuarios/listar').then(function (response) {

        console.log(response.data);

        // Pegando cada usuário
        for (var i = 0; i < response.data.length; i++) {
            if (emailUsuario == response.data[i].email) {

                // IdUsuairo
                var user = response.data[i].idUsuario;

                console.log(user);

                // Verificando se esse usuário já tem pedido
                axios.get('/api/pedidos/listar').then(function (response) {

                    var possui = false;

                    for (var i = 0; i < response.data.length; i++) {
                        if (user == response.data[i].IdUsuario) {

                            possui = true;

                            // Adicionando ao carrinho
                            adicionarPedidosHamburgueres(response, i);

                            break;

                        }
                    }

                    if (!possui) {
                        adicionarPedidos(nome, preco, user);
                        var num = posicaoUsuarioEPedidos(user, response);
                        adicionarPedidosHamburgueres(response, num);

                    }
                });
                break;
            }
        }

    });
}
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


function toggleDropdown() {
    const menu = document.getElementById("dropdownMenu");
    menu.style.display = menu.style.display === "block" ? "none" : "block";
}

// Fecha se clicar fora
window.onclick = function (event) {
    if (!event.target.matches('.btn-filtrar, .btn-filtrar *')) {
        const dropdowns = document.getElementsByClassName("dropdown-content");
        for (let i = 0; i < dropdowns.length; i++) {
            dropdowns[i].style.display = "none";
        }
    }
}

function filtrar(categoria) {
    const secoes = document.querySelectorAll(".section");
    secoes.forEach(secao => {
        if (categoria === "todos") {
            secao.style.display = "block";
        } else if (secao.querySelector("h2").innerText.toLowerCase().includes(categoria)) {
            secao.style.display = "block";
        } else {
            secao.style.display = "none";
        }
    });
}

function mostrarBotaoLogin() {
    let interface = document.querySelector(".interface");

    interface.classList.add("mostrar");
    console.log(interface.className);
}

function adicionarPedidosHamburgueres(IdPedido, IdHamburguer) {
    var url = '/api/pedidohamburgueres/create';

    console.log(IdPedido);

    const dados = {
        IdPedido: IdPedido,
        IdHamburguer: IdHamburguer
    }

    console.log(dados);

    axios.post(url, dados)
        .then(response => {
            console.log('Sucesso! Resposta do servidor:', response.data);
        })
        .catch(error => {
            console.error('Erro ao adicionar pedido:', error);
        });
}

function adicionarPedidos(nome, preco, user, idHamburguer) {
    const dados = {
        nome: "Pedido_" + user + nome,
        PrecoTotal: preco,
        estado: "Decidindo",
        IdUsuario: user
    };

    var url = '/api/pedidos/create';

    axios.post(url, dados)
        .then(response => {
            console.log('Criou pedidos', response.data);

            adicionarPedidosHamburgueres(response.data.idPedido, idHamburguer)
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

                        console.log(response.data[i])

                        if (user == response.data[i].idUsuario) {

                            possui = true;

                            console.log("Adicionando novo hamburguer ao pedido");

                            // Variaveis do pedido
                            var idPedido = response.data[i].idPedido;
                            var precoAnterior = response.data[i].precoTotal;
                            var novosDados = {
                                idPedido: idPedido,
                                nome: response.data[i].nome,
                                precoTotal: precoAnterior + response.data[i].precoTotal,
                                estado: response.data[i].estado,
                                idUsuario: response.data[i].idUsuario
                            }

                            // Adicionando ao carrinho
                            adicionarPedidosHamburgueres(idPedido, id);

                            
                            console.log(idPedido)

                            axios.put(`/api/pedidos/update/${idPedido}`, novosDados)
                                .then(response => {
                                    console.log('Atualizou o preço total', response.data);
                                })
                                .catch(error => {
                                    console.error('Erro ao atualizar valor:', error);
                                });

                            break;

                        }
                    }

                    if (!possui) {
                        adicionarPedidos(nome, preco, user, id);

                    }
                });
                break;
            }
        }

    });
}

document.addEventListener('DOMContentLoaded', function () {
    const loginBtn = document.querySelector('.BottomRightBox a');
    const outBox = document.querySelector('.OutBox');

    if (loginBtn && outBox) {
        loginBtn.addEventListener('click', function (e) {
            e.preventDefault();
            outBox.classList.add('swap');

            for (let i = 1; i <= 4; i++){
                if (i == 1 || i == 3) {
                    document.querySelector(`.Ball${i}`).classList.add('swap');
                }
               
            }

            // Opcional: Redireciona após a animação
            setTimeout(() => {
                window.location.href = loginBtn.href;
            }, 800);
        });
    }
});


/*

        BANNER DO HOME

*/

document.addEventListener('DOMContentLoaded', function () {
    const sections = [
        document.getElementById('BannerContent'),
        document.getElementById('OfertasDiarias'),
        document.getElementById('RecomendacoesDoChefe'),
        document.getElementById('Cardapio')
    ];
    let currentSection = 0;
    let isScrolling = false;

   
    if (currentSection === 3) {
        sec.classList.add('active');
    } else {
        sec.classList.remove('active');
    }

    updateActiveSection();

    window.addEventListener('wheel', function (e) {
        if (isScrolling) return;

        if (e.deltaY > 0 && currentSection < sections.length - 1) {
            currentSection++;
            isScrolling = true;
            sections[currentSection].scrollIntoView({ behavior: 'smooth', block: 'center' });
            updateActiveSection();
            setTimeout(() => { isScrolling = false; }, 700);
            e.preventDefault();
        } else if (e.deltaY < 0 && currentSection > 0) {
            currentSection--;
            isScrolling = true;
            sections[currentSection].scrollIntoView({ behavior: 'smooth', block: 'center' });
            updateActiveSection();
            setTimeout(() => { isScrolling = false; }, 700);
            e.preventDefault();
        }
    }, { passive: false });
});



            /*
            
                LOGIN E REGISTRO
            
            */

document.getElementById('registerForm').addEventListener('submit', function (e) {
    const btn = document.getElementById('registerSubmit');
    const text = btn.querySelector('.button-text');
    const spinner = btn.querySelector('.spinner');

    text.style.display = 'none';
    spinner.style.display = 'inline-block';

    btn.disabled = true;
});
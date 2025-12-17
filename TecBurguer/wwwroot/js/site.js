/*
    --------------------------------------------------------
    NOVAS FUNÇÕES PARA CARREGAR DETALHES DO PEDIDO (AJAX/AXIOS)
    --------------------------------------------------------
*/

// Função para formatar números como moeda Real Brasileiro (R$ 0,00)
function formatarMoeda(valor) {
    const numero = parseFloat(valor);
    if (isNaN(numero)) {
        return 'R$ 0,00';
    }
    // Usa toFixed(2) e replace para simular formatação de moeda
    return `R$ ${numero.toFixed(2).replace('.', ',')}`;
}

/**
 * Busca os detalhes de um pedido específico via API e injeta o HTML no container.
 */
async function carregarDetalhesPedido(pedidoId) {
    const detalhesContainer = document.getElementById(`detalhesPedido_${pedidoId}`);
    const collapseElement = document.getElementById(`collapseDetalhes_${pedidoId}`);

    // Previne recarregar se o painel estiver visível (o Bootstrap Collapse cuida do toggle)
    if (collapseElement.classList.contains('show')) {
        return;
    }

    // Exibe um spinner de carregamento enquanto aguarda a API
    detalhesContainer.innerHTML = `
        <div class="text-center py-3">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Carregando...</span>
            </div> 
            Carregando detalhes do pedido #${pedidoId}...
        </div>
    `;

    try {
        // Rota API Assumida: /Pedido/DetalhesAPI/{id}
        const response = await axios.get(`/Pedido/DetalhesAPI/${pedidoId}`);
        const pedidoDetalhes = response.data;

        let htmlContent = `
            <h5 class="card-title mb-3">Itens do Pedido #${pedidoDetalhes.idPedido}</h5>
            <ul class="list-group list-group-flush mb-3">
        `;

        let totalCalculado = 0;

        pedidoDetalhes.pedidoHamburgueres.forEach(item => {
            // Corrigido para verificar precoUnitario (mais comum) e hamburguerPreco
            const precoUnitario = item.precoUnitario || item.hamburguerPreco || 0;
            const subtotal = item.quantidade * precoUnitario;
            totalCalculado += subtotal;

            htmlContent += `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${item.hamburguerNome}</strong> 
                        <small class="text-muted"> (${formatarMoeda(precoUnitario)} cada)</small>
                        <span class="badge bg-secondary ms-2">${item.quantidade}x</span>
                    </div>
                    <span class="fw-bold">${formatarMoeda(subtotal)}</span>
                </li>
            `;
        });

        htmlContent += `</ul>
            <hr>
            <div class="d-flex justify-content-between fw-bold fs-5 mb-0">
                <span>Total:</span>
                <span class="text-success">${formatarMoeda(totalCalculado)}</span>
            </div>
        `;

        detalhesContainer.innerHTML = htmlContent;

    } catch (error) {
        console.error("Erro ao carregar detalhes do pedido:", error.response?.data || error.message);
        detalhesContainer.innerHTML = '<div class="alert alert-danger mb-0">❌ Não foi possível carregar os detalhes do pedido. Verifique o console ou a API `/Pedido/DetalhesAPI/`</div>';
    }
}

// Torna a nova função acessível globalmente
window.carregarDetalhesPedido = carregarDetalhesPedido;


/*
    --------------------------------------------------------
    SEU CÓDIGO JS EXISTENTE
    --------------------------------------------------------
*/


// Funções Comentadas (Originalmente desativadas/em jQuery)
//$("#Cep").blur(function () {
//    var cep = $(this).val().replace(/\D/g, '');
//
//    if (cep.length === 8) {
//        $.getJSON(`https://viacep.com.br/ws/${cep}/json/`, function (data) {
//            if (!("erro" in data)) {
//                $("#Rua").val(data.logradouro);
//                $("#Bairro").val(data.bairro);
//                $("#Cidade").val(data.localidade);
//            } else {
//                alert("CEP não encontrado.");
//
//            }
//        });
//    }
//});

//$("#Localidade").blur(function(){
//    var localidade = $(this).val();
//
//    if(localidade == "Condomínio"){
//        $("#NumeroCasaLabel").text("Nome do condomínio / Complemento")
//    } else {
//        $("#NumeroCasaLabel").text("Numero da casa")
//    }
//});


function filtrar(categoria) {
    document.querySelectorAll(".section").forEach(secao => {
        const titulo = secao.querySelector("h2").innerText.toLowerCase();
        const mostrar = categoria === "todos" || titulo.includes(categoria);
        secao.style.display = mostrar ? "block" : "none";
    });
}

function mostrarBotaoLogin() {
    const interfaceBox = document.getElementById("FacaLogin");
    interfaceBox.classList.add("mostrar");
    console.log(interfaceBox.className);
}

// Funções Comentadas (AumentarQuantidadeHamburguerPedido)
//function AumentarQuantidadeHamburguerPedido(IdPedidoHamburguer, preco) {
//    axios.get('/api/pedidohamburgueres/listar').then(resPH => {
//        const itemExistente = resPH.data.find(item => item.id === IdPedidoHamburguer);
//
//        if (itemExistente) {
//            const dadosPH = {
//                id: IdPedidoHamburguer,
//                idPedido: itemExistente.idPedido,
//                idHamburguer: itemExistente.idHamburguer,
//                quantidade: itemExistente.quantidade + 1
//            };
//
//            axios.put(`/api/pedidohamburgueres/update/${itemExistente.id}`, dadosPH)
//                .then(res => console.log('Quantidade do item aumentada:', res.data))
//                .catch(err => console.error('Erro ao atualizar quantidade:', err));
//
//            axios.get('/api/pedidos/listar').then(resP => {
//                const pedidoExistente = resP.data.find(p => p.idPedido === itemExistente.idPedido);
//
//                if (pedidoExistente) {
//
//                    const novosDadosPedido = {
//                        idPedido: pedidoExistente.idPedido,
//                        nome: pedidoExistente.nome,
//                        precoTotal: pedidoExistente.precoTotal + preco,
//                        estado: pedidoExistente.estado,
//                        idUsuario: pedidoExistente.idUsuario
//                    };
//
//                    axios.put(`/api/pedidos/update/${pedidoExistente.idPedido}`, novosDadosPedido)
//                        .then(res => location.reload())
//                        .catch(err => console.error('Erro ao atualizar valor total:', err));
//                }
//            });
//        }
//    });
//}

function showLoading() {
    const overlay = document.getElementById('LoadingOverlay');
    const reactor = document.querySelector('.reactor-loader');

    overlay.classList.add('active');

    loadingInterval = setInterval(() => {
        reactor.classList.toggle('show-cart');
    }, 1200);
}

function RemoverQuantidadeBebidaPedido(IdPedidoBebida) {
    showLoading();
    axios.get('/api/pedidobebidas/listar').then(response => {
        response.data.forEach(item => {
            if (item.id == IdPedidoBebida) {
                const dados = {
                    id: IdPedidoBebida,
                    idPedido: item.idPedido,
                    idBebidas: item.idBebidas,
                    quantidade: item.quantidade - 1
                };

                if (dados.quantidade <= 0) {
                    axios.delete(`/api/pedidobebidas/delete/${item.id}`)
                        .then(res => { CalcularPrecoTotal(item.idPedido); })
                        .catch(err => { location.reload(); console.error(err); });
                } else {
                    axios.put(`/api/pedidobebidas/update/${item.id}`, dados)
                        .then(res => { CalcularPrecoTotal(item.idPedido); })
                        .catch(err => { location.reload(); console.error(err); });
                }
            }
        });
    }).catch(err => console.log(err));
}

function RemoverQuantidadeHamburguerPedido(IdPedidoHamburguer) {
    showLoading();
    axios.get('/api/pedidohamburgueres/listar').then(response => {
        response.data.forEach(item => {
            if (item.id == IdPedidoHamburguer) {
                const dados = {
                    id: IdPedidoHamburguer,
                    idPedido: item.idPedido,
                    idHamburguer: item.idHamburguer,
                    quantidade: item.quantidade - 1
                };

                if (dados.quantidade <= 0) {
                    axios.delete(`/api/pedidohamburgueres/delete/${item.id}`)
                        .then(res => {
                            CalcularPrecoTotal(item.idPedido);
                        })
                        .catch(err => console.error(err));

                    
                } else {
                    axios.put(`/api/pedidohamburgueres/update/${item.id}`, dados)
                        .then(res => { CalcularPrecoTotal(item.idPedido); })
                        .catch(err => console.error(err));
                }
            }
        });
    });
}

async function CalcularPrecoTotal(idPedido) {
    console.log('Calculando preço total para pedido:', idPedido);

    try {
        const [resHamburgueres, resBebidas, resOfertas] = await Promise.all([
            axios.get(`/api/pedidohamburgueres/ListarPorPedido/${idPedido}`),
            axios.get(`/api/pedidobebidas/ListarPorPedido/${idPedido}`),
            axios.get(`/api/ofertasapi/listar`)
        ]);

        const listaHamburgueres = resHamburgueres.data;
        const listaBebidas = resBebidas.data;
        const listaOfertas = resOfertas.data;

        console.table(listaOfertas);

        if (listaHamburgueres.length === 0 && listaBebidas.length === 0) {
            console.warn("Pedido vazio. Excluindo pedido...");

            await axios.delete(`/api/pedidos/delete/${idPedido}`);

            window.location.href = "/";
            return; 
        }

        let precoTotalCalculado = 0;

        listaHamburgueres.forEach(item => {

            console.log(item);

            let precoReal = item.precoUnitario;

            const ofertaAtiva = listaOfertas.find(o => o.idHamburguer == item.idHamburguer);

            if (ofertaAtiva) {
                const agora = new Date();

                const dataLocal = new Date(agora.getTime() - (agora.getTimezoneOffset() * 60000));

                const dataFormatada = dataLocal.toISOString().slice(0, 19);

                if (dataFormatada < ofertaAtiva.dataTermino) {
                    precoReal = ofertaAtiva.precoFinal;
                    console.log(`Oferta aplicada no ${item.nomeHamburguer}: De ${item.precoUnitario} por ${precoReal}`);
                }
            }

            precoTotalCalculado += (item.quantidade * precoReal);
        });

        // 4. Calcula total das Bebidas
        listaBebidas.forEach(item => {
            precoTotalCalculado += (item.quantidade * item.precoUnitario);
        });

        // Arredonda para 2 casas decimais
        const precoFinal = Math.round(precoTotalCalculado * 100) / 100;

        console.log('Preço Total Final Calculado:', precoFinal);

        // 5. Atualiza o Pedido no Banco de Dados
        const novosDados = {
            precoTotal: precoFinal
        };

        await axios.patch(`/api/pedidos/update/${idPedido}`, novosDados);

        console.log('Banco de dados atualizado com sucesso.');
        location.reload();

    } catch (err) {
        console.error('Erro ao calcular/atualizar total:', err);
        alert('Ocorreu um erro ao atualizar o carrinho.');
    }
}


function adicionarPedidosHamburgueres(idPedido, idHamburguer, update) {
    const url = '/api/pedidohamburgueres/create';
    const dados = { IdPedido: idPedido, IdHamburguer: idHamburguer, quantidade: 1 };

    axios.get('/api/pedidohamburgueres/listar').then(response => {
        console.log('Items do carrinho recebidos:', response.data);

        let criar = true;

        response.data.forEach(item => {

            if (idHamburguer == item.idHamburguer && item.idPedido === idPedido) {
                criar = false;
                dados.id = item.id;
                dados.quantidade = item.quantidade + 1;

                console.log('Item já existe, atualizando quantidade:', dados);

                axios.put(`/api/pedidohamburgueres/update/${item.id}`, dados)
                    .then(res => {
                        console.log('Quantidade atualizada com sucesso');
                        CalcularPrecoTotal(idPedido, update);
                    })
                    .catch(err => {
                        if (err.response && err.response.status === 400) {

                            const mensagemDoServidor = err.response.data;

                            alert("⚠️ ATENÇÃO: " + mensagemDoServidor);

                            location.reload();
                        }
                        else {
                            alert("Ocorreu um erro inesperado ao atualizar o pedido.");
                            location.reload();
                        }
                    });
            }
        });

        if (criar) {
            console.log('Criando novo item:', dados);

            axios.post(url, dados)
                .then(res => {
                    console.log('Item criado com sucesso:', res.data);
                    CalcularPrecoTotal(idPedido, update);
                })
                .catch(err => {
                    console.error('Erro ao criar pedido:', err.response?.data || err.message);
                    alert('Erro ao adicionar item. Por favor, tente novamente.');
                });
        }
    }).catch(err => {
        console.error('Erro ao buscar items do carrinho:', err);
        alert('Erro ao buscar carrinho. Por favor, tente novamente.');
    });
}

function adicionarPedidos(nome, preco, idUsuario, idHamburguer, tipo) {
    const url = '/api/pedidos/create';

    const code = Math.floor(100000 + Math.random() * 900000).toString();

    const dados = {
        nome: `${code}`,
        PrecoTotal: parseFloat(preco),
        estado: "Decidindo",
        IdUsuario: idUsuario
    };

    console.log('Criando novo pedido:', dados);

    axios.post(url, dados)
        .then(response => {
            console.log('Pedido criado com sucesso:', response.data);

            console.log('O tipo é:, tipo);

            if (tipo == 'Bebida') {
                adicionarPedidosBebidas(response.data.idPedido, idHamburguer);
                
            }
            else {
                adicionarPedidosHamburgueres(response.data.idPedido, idHamburguer);
            }

        })
        .catch(err => {
            console.error('Erro ao criar pedido:', err.response?.data || err.message);
            alert('Erro ao criar pedido. Por favor, tente novamente.');
        });
}

function adicionarPedidosBebidas(idPedido, idBebida) {
    const url = '/api/pedidobebidas/create';

    console.log("id da bebida: " + idBebida);
    const dados = {
        IdPedido: idPedido,
        IdBebidas: idBebida,
        quantidade: 1
    };

    axios.get('/api/pedidobebidas/listar').then(response => {

        let criar = true;

        response.data.forEach(item => {

            if (idBebida == item.idBebidas && item.idPedido === idPedido) {
                criar = false;
                dados.id = item.id;
                dados.quantidade = item.quantidade + 1;

                axios.put(`/api/pedidobebidas/update/${item.id}`, dados)
                    .then(res => {
                        console.log('Quantidade atualizada com sucesso');
                        CalcularPrecoTotal(idPedido);
                    })
                    .catch(err => {
                        if (err.response && err.response.status === 400) {

                            const mensagemDoServidor = err.response.data;

                            alert("⚠️ ATENÇÃO: " + mensagemDoServidor);

                            location.reload();
                        }
                        else {
                            alert("Ocorreu um erro inesperado ao atualizar o pedido.");
                            location.reload();
                        }
                    });
            }
        });

        if (criar) {
            console.log('Criando novo item:', dados);

            axios.post(url, dados)
                .then(res => {
                    console.log('Item criado com sucesso:', res.data);
                    CalcularPrecoTotal(idPedido);
                })
                .catch(err => {
                    console.error('Erro ao criar pedido:', err.response?.data || err.message);
                    alert('Erro ao adicionar item. Por favor, tente novamente.');
                });
        }
    }).catch(err => {
        console.error('Erro ao buscar items do carrinho:', err);
        alert('Erro ao buscar carrinho. Por favor, tente novamente.');
    });
}

function adicionarAoCarrinho(nome, preco, idHamburguer, emailUsuario, tipo) {

    axios.get('/api/usuarios/listar').then(response => {
        console.log('Usuários recebidos:', response.data);

        const usuario = response.data.find(u => u.email === emailUsuario);

        if (!usuario) {
            console.error('Usuário não encontrado com email:', emailUsuario);
            alert('Erro: Usuário não encontrado');
            return;
        }

        console.log('Usuário encontrado:', usuario);
        const idUsuario = usuario.idUsuario;

        axios.get('/api/pedidos/listar').then(response => {
            console.log('Pedidos recebidos:', response.data);

            const pedidoExistente = response.data.find(p => p.idUsuario === idUsuario);

            if (pedidoExistente) {
                console.log('Pedido existente encontrado:', pedidoExistente);

                if (pedidoExistente.estado != "Decidindo") {
                    document.getElementById("FacaLogin").classList.add("mostrar");
                } else {
                    showLoading();

                    if (tipo == "Bebida") {
                        adicionarPedidosBebidas(pedidoExistente.idPedido, idHamburguer);
                    }
                    else {
                        adicionarPedidosHamburgueres(pedidoExistente.idPedido, idHamburguer);
                    }
                    
                }
                

            } else {
                console.log('Nenhum pedido existente, criando novo');
                showLoading();
                adicionarPedidos(nome, preco, idUsuario, idHamburguer, tipo);
            }

        }).catch(err => {
            console.error('Erro ao buscar pedidos:', err);
            alert('Erro ao buscar pedidos. Por favor, tente novamente.');
        });

    }).catch(err => {
        console.error('Erro ao buscar usuários:', err);
        alert('Erro ao buscar usuários. Por favor, tente novamente.');
    });
}

document.addEventListener('DOMContentLoaded', () => {
    const loginBtn = document.querySelector('.BottomRightBox a');
    const outBox = document.querySelector('.OutBox');
    const forget = document.getElementById("forgot-password");

    if (loginBtn && outBox) {

        loginBtn.addEventListener('click', e => {

            e.preventDefault();

            outBox.classList.add('swap');
            [1, 3].forEach(i => {
                document.querySelector(`.Ball${i}`).classList.add('swap');
            });

            setTimeout(() => {
                location.href = loginBtn.href;
            }, 800);
        });
    }

    if (forget && outBox) {
        forget.addEventListener('click', e => {
            e.preventDefault();

            outBox.classList.add('swap');
            [1, 3].forEach(i => {
                document.querySelector(`.Ball${i}`).classList.add('swap');
            });

            document.getElementById("LeftBox").innerHTML = `
<form>
    <div>
        <h2 class="TextoComImagem2">Recuperar senha</h2>
        <p>Digite o seu email</p>

        <div class="input-icon-group">
            <span class="input-icon">
                <!-- Ícone de email -->
                <svg width="20" height="20" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4zm2-1a1 1 0 0 0-1 1v.217l7 4.2 7-4.2V4a1 1 0 0 0-1-1H2zm13 2.383-4.708 2.825L15 11.383V5.383zm-.034 6.434-5.966-3.583-5.966 3.583A1 1 0 0 0 2 13h12a1 1 0 0 0 .966-1.183zM1 5.383v6l4.708-3.175L1 5.383z" />
                </svg>
            </span>
            <div class="form-floating">
                <input class="form-control" placeholder=" "/>
                <label>Email</label>
            </div>
        </div>
    </div>

    <button id="registerSubmit" type="submit">Verificar conta</button>
</form > `

            setTimeout(() => {
                location.href = forget.href;
            }, 800);
        });
    }
});

/*
    RECOMENDAÇÕES DO CHEFE
*/

document.addEventListener("DOMContentLoaded", () => {
    const wrapper = document.querySelector(".carrossel-wrapper");
    const slides = document.querySelectorAll(".item-recomendacao");
    let index = 0;

    if (slides.length === 0) return;

    function showSlide(i) {
        const offset = -i * 100;
        wrapper.style.transform = `translateX(${offset}%)`;
    }

    function nextSlide() {
        index = (index + 1) % slides.length;
        showSlide(index);
    }
    function prevSlide() {
        index = (index - 1 + slides.length) % slides.length;
        showSlide(index);
    }

    window.nextSlide = nextSlide;
    window.prevSlide = prevSlide;

    showSlide(index);

    setInterval(nextSlide, 15000);
});

/*
    CARRINHO
*/

let tipoMoradia = "casa";

function AlternarMoradia(tipo) {
    tipoMoradia = tipo;

    // Atualiza botões
    document.getElementById('btnCasa').classList.toggle('active', tipo === 'casa');
    document.getElementById('btnApto').classList.toggle('active', tipo === 'apto');

    // Troca os inputs visíveis
    if (tipo == 'casa') {
        document.getElementById('areaCasa').style.display = 'block';
        document.getElementById('areaApto').style.display = 'none';
    } else {
        document.getElementById('areaCasa').style.display = 'none';
        document.getElementById('areaApto').style.display = 'block';
    }
}

// --- Busca de CEP (ViaCEP) ---
function BuscarEndereco(cep) {
    const cleanCep = cep.replace(/\D/g, '');

    if (cleanCep.length === 8) {
        document.getElementById('cepInput').disabled = true;

        axios.get(`https://viacep.com.br/ws/${cleanCep}/json/`)
            .then(response => {
                const data = response.data;
                if (!data.erro) {
                    document.getElementById('ruaInput').value = data.logradouro;
                    document.getElementById('bairroInput').value = data.bairro;
                    document.getElementById('cidadeInput').value = data.localidade;
                    document.getElementById('ufInput').value = data.uf;
                    document.getElementById('msgErroCep').style.display = 'none';

                    if (tipoMoradia == 'casa') document.getElementById('numCasaInput').focus();
                    else document.getElementById('condominioInput').focus();
                } else {
                    document.getElementById('msgErroCep').style.display = 'block';
                    document.getElementById('msgErroCep').innerText = "CEP não encontrado.";
                }
            })
            .catch(err => {
                console.error(err);
                document.getElementById('msgErroCep').style.display = 'block';
                document.getElementById('msgErroCep').innerText = "Erro ao buscar CEP.";
            })
            .finally(() => {
                document.getElementById('cepInput').disabled = false;
            });
    }
}

function FinalizarCompra(username, idPedido) {

    axios.get('/api/usuarios/listar').then(response => {
        console.log(username);

        const usuarioExistente = response.data.find(p => p.email == username);

        if (usuarioExistente) {

            if (document.getElementById("cepInput").value != '' && (usuarioExistente.cep == null || usuarioExistente.cep == "")) {

                const cep = document.getElementById('cepInput').value;
                const rua = document.getElementById('ruaInput').value;
                const bairro = document.getElementById('bairroInput').value;
                const cidade = document.getElementById('cidadeInput').value;
                const uf = document.getElementById('ufInput').value;

                if (!rua) {
                    alert("Por favor, informe um CEP válido primeiro.");
                    return;
                }

                let complementoFinal = "";

                if (tipoMoradia == 'casa') {
                    const numero = document.getElementById('numCasaInput').value;
                    if (!numero) { alert("Informe o número da casa."); return; }
                    complementoFinal = `${numero}`;
                } else {
                    const cond = document.getElementById('condominioInput').value;
                    const bloco = document.getElementById('blocoInput').value;
                    const andar = document.getElementById('andarInput').value;
                    const numApto = document.getElementById('numAptoInput').value;

                    if (!cond || !numApto) { alert("Informe pelo menos o Condomínio e o Número do Apto."); return; }

                    complementoFinal = `Cond. ${cond}, Bloco ${bloco}, Andar ${andar}, Apto ${numApto}`;
                }

                const dados = {

                    Cep: document.getElementById("cepInput").value,
                    Rua: rua,
                    Bairro: bairro,
                    Cidade: `${cidade}`,
                    Estado: uf,
                    Complemento: complementoFinal
                }

                axios.patch(`/api/usuarios/update/${usuarioExistente.idUsuario}`, dados);

                const novosDados = {
                    estado: "Cozinhando",
                };

                axios.patch(`/api/pedidos/update/${idPedido}`, novosDados)
                    .then(res => {
                        location.reload();

                    })
                    .catch(err => console.error('Erro ao atualizar valor:', err));

                return;
            }

            console.log(usuarioExistente.cep)

            if (usuarioExistente.cep == null || usuarioExistente.cep == "") {
                document.getElementById("PopUpNaoTemCEP").classList.add("Aparecer");
                console.log('apareceu')
            } else {
                const novosDados = {
                    estado: "Cozinhando",
                };

                axios.patch(`/api/pedidos/update/${idPedido}`, novosDados)
                    .then(res => {
                        location.reload();
                    })
                    .catch(err => console.error('Erro ao atualizar valor:', err));
            }
        }
    });
}

function FecharPopUp() {
    document.getElementById("PopUpNaoTemCEP").classList.remove("Aparecer");
}

function fecharPopup() {
    document.getElementById("FacaLogin").classList.remove("mostrar");
}

const dropdown = document.getElementById("dropdown-admin");
const menu = document.getElementById("menuDrop");
let fixo = false;
let dropHover = false;

dropdown.addEventListener("mouseenter", function() {
    menu.classList.add("show");
    dropHover = true;
});

menu.addEventListener("mouseenter", function () {
    fixo = true;

    if (dropHover) {
        menu.classList.add("show");
    }
});

menu.addEventListener("mouseleave", function () {
    fixo = false;
    dropHover = false;
    menu.classList.remove("show");
});

dropdown.addEventListener("mouseleave", function() {
    if (fixo) { return}
    menu.classList.remove("show");
    
});

dropdown.addEventListener("click", function() {
    if (menu.classList.contains("show")) {
        menu.classList.remove("show");
    } else {
        menu.classList.add("show");
    }
});


/*
    LOGIN E REGISTRO
*/

document.addEventListener("DOMContentLoaded", function () {

    const form = $('#registerForm');

    form.on('submit', function (e) {

        if (!form.valid()) {
            return;
        }

        const btn = document.getElementById('registerSubmit');
        const text = btn.querySelector('.button-text');
        const spinner = btn.querySelector('.spinner');

        text.style.display = 'none';
        spinner.style.display = 'inline-block';
        btn.disabled = true;
    });
});
/*
    PEDIDO VENDEDOR.
*/




// ==========================================================
// ESTE É O BLOCO DOMContentLoaded DO CARDÁPIO, ONDE ESTÁ A FUNÇÃO DE FILTRO
// A função 'filtrarPorCategoria' foi corrigida com um offset de 220px.
// ==========================================================
document.addEventListener('DOMContentLoaded', function () {
    const linksCategoria = document.querySelectorAll('.LinkCategoria');
    const todosOsBlocos = document.querySelectorAll('.BlocoHamburguer');
    const todosOsTitulos = document.querySelectorAll('.TituloCategoria');

    function filtrarPorCategoria(categoriaAlvo) {

        // --- 1. Lógica de Filtro (Mantida) ---
        todosOsTitulos.forEach(titulo => titulo.style.display = 'none');
        todosOsBlocos.forEach(bloco => bloco.style.display = 'none');
        linksCategoria.forEach(link => {
            if (link.dataset.categoria === categoriaAlvo) {
                link.classList.add('ativo');
            } else {
                link.classList.remove('ativo');
            }
        });

        if (categoriaAlvo === 'Todos') {
            todosOsTitulos.forEach(titulo => titulo.style.display = 'inline-block');
            todosOsBlocos.forEach(bloco => bloco.style.display = 'block');
        } else {
            document.getElementById('pesquisa').value = '';
            const tituloAlvo = document.getElementById(`Categoria_${categoriaAlvo}`);
            if (tituloAlvo) {
                tituloAlvo.style.display = 'inline-block';
            }

            document.querySelectorAll(`[data-categoria-item='${categoriaAlvo}']`).forEach(bloco => {
                bloco.style.display = 'block';
            });
        }

        // --- 2. Lógica de Rolagem (CORRIGIDA COM NOVO OFFSET) ---
        const primeiroTituloVisivel = document.querySelector('.TituloCategoria[style*="inline-block"]');

        if (primeiroTituloVisivel) {
            // Offset aumentado para 220px para compensar a altura dos cabeçalhos/filtros fixos.
            const offset = 220;

            // Calcula a posição de rolagem ajustada:
            const targetPosition = primeiroTituloVisivel.getBoundingClientRect().top + window.scrollY - offset;

            // Rola para a nova posição.
            window.scrollTo({
                top: targetPosition,
                behavior: 'smooth'
            });
        }
    }

    linksCategoria.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const categoria = this.dataset.categoria;
            filtrarPorCategoria(categoria);
        });
    });
    // Garante que o filtro 'Todos' seja carregado na inicialização
    filtrarPorCategoria('Todos');
});

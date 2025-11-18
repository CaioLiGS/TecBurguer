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

// Fecha dropdown ao clicar fora
window.onclick = function (event) {
    if (!event.target.matches('.btn-filtrar, .btn-filtrar *')) {
        document.querySelectorAll(".dropdown-content").forEach(dropdown => {
            dropdown.style.display = "none";
        });
    }
};

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

//function AumentarQuantidadeHamburguerPedido(IdPedidoHamburguer, preco) {
//    axios.get('/api/pedidohamburgueres/listar').then(resPH => {
//        const itemExistente = resPH.data.find(item => item.id === IdPedidoHamburguer);

//        if (itemExistente) {
//            const dadosPH = {
//                id: IdPedidoHamburguer,
//                idPedido: itemExistente.idPedido,
//                idHamburguer: itemExistente.idHamburguer,
//                quantidade: itemExistente.quantidade + 1
//            };

//            axios.put(`/api/pedidohamburgueres/update/${itemExistente.id}`, dadosPH)
//                .then(res => console.log('Quantidade do item aumentada:', res.data))
//                .catch(err => console.error('Erro ao atualizar quantidade:', err));

//            axios.get('/api/pedidos/listar').then(resP => {
//                const pedidoExistente = resP.data.find(p => p.idPedido === itemExistente.idPedido);

//                if (pedidoExistente) {

//                    const novosDadosPedido = {
//                        idPedido: pedidoExistente.idPedido,
//                        nome: pedidoExistente.nome,
//                        precoTotal: pedidoExistente.precoTotal + preco,
//                        estado: pedidoExistente.estado,
//                        idUsuario: pedidoExistente.idUsuario
//                    };

//                    axios.put(`/api/pedidos/update/${pedidoExistente.idPedido}`, novosDadosPedido)
//                        .then(res => location.reload())
//                        .catch(err => console.error('Erro ao atualizar valor total:', err));
//                }
//            });
//        }
//    });
//}

function RemoverQuantidadeHamburguerPedido(IdPedidoHamburguer) {
    const url = `/api/pedidohamburgueres/update/${IdPedidoHamburguer}`;
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
                            CalcularPrecoTotal(item.idPedido)
                        })
                        .catch(err => console.error('Erro ao atualizar pedido:', err));
                }
                else {

                    axios.put(`/api/pedidohamburgueres/update/${item.id}`, dados)
                        .then(res => {
                            CalcularPrecoTotal(item.idPedido)
                        })
                        .catch(err => console.error('Erro ao atualizar pedido:', err));
                }
            }
        });
    });
}

function CalcularPrecoTotal(idPedido) {

    console.log('Calculando preço total para pedido:', idPedido);

    const novosDados = {
        precoTotal: 0,
    };

    axios.get(`/api/pedidohamburgueres/ListarPorPedido/${idPedido}`)
        .then(response => {
            console.log('Items do pedido recebidos:', response.data);

            let precoTotalCalculado = 0;

            response.data.forEach(item => {
                const subtotal = (item.quantidade * item.precoUnitario);
                precoTotalCalculado += subtotal;
                console.log(`Item: ${item.nomeHamburguer}, Qtd: ${item.quantidade}, Preço Unitário: ${item.precoUnitario}, Subtotal: ${subtotal}`);
            });

            novosDados.precoTotal = Math.round(precoTotalCalculado * 100) / 100;
            
            console.log('Preço total calculado:', novosDados.precoTotal);

            axios.patch(`/api/pedidos/update/${idPedido}`, novosDados)
                .then(res => {
                    console.log('Pedido atualizado com sucesso');
                    location.reload();

                    setTimeout(function () {
                        document.querySelector('.interface').classList.add('mostrar');
                    }, 1000);

                    setTimeout(function () {
                        document.querySelector('.interface').classList.remove('mostrar');
                    }, 4000); 
                })
                .catch(err => {
                    console.error('Erro ao atualizar pedido:', err.response?.data || err.message);
                });
        })
        .catch(err => {
            console.error('Erro ao buscar items do pedido:', err);
            alert('Erro ao calcular total. Por favor, tente novamente.');
        });
}

function adicionarPedidosHamburgueres(idPedido, idHamburguer, update) {
    const url = '/api/pedidohamburgueres/create';
    const dados = { IdPedido: idPedido, IdHamburguer: idHamburguer, quantidade: 1 };

    console.log('Iniciando adicionarPedidosHamburgueres:', { idPedido, idHamburguer });

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
                        console.error('Erro ao atualizar item do pedido:', err.response?.data || err.message);
                        alert('Erro ao atualizar item. Por favor, tente novamente.');
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

function adicionarPedidos(nome, preco, idUsuario, idHamburguer) {
    const url = '/api/pedidos/create';
    
    const dados = {
        nome: `Pedido_${idUsuario}_${nome}`,
        PrecoTotal: parseFloat(preco),
        estado: "Decidindo",
        IdUsuario: idUsuario
    };

    console.log('Criando novo pedido:', dados);

    axios.post(url, dados)
        .then(response => {
            console.log('Pedido criado com sucesso:', response.data);
            adicionarPedidosHamburgueres(response.data.idPedido, idHamburguer);
        })
        .catch(err => {
            console.error('Erro ao criar pedido:', err.response?.data || err.message);
            alert('Erro ao criar pedido. Por favor, tente novamente.');
        });
}

function adicionarAoCarrinho(nome, preco, idHamburguer, emailUsuario, update = false) {

    console.log('Iniciando adicionarAoCarrinho', { nome, preco, idHamburguer, emailUsuario, update });

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
                adicionarPedidosHamburgueres(pedidoExistente.idPedido, idHamburguer, update);

            } else {
                console.log('Nenhum pedido existente, criando novo');
                adicionarPedidos(nome, preco, idUsuario, idHamburguer);
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

    if (loginBtn && outBox) {

        loginBtn.addEventListener('click', e => {

            e.preventDefault();

            outBox.classList.add('swap');
            [1, 3].forEach(i => {
                document.querySelector(`.Ball${i}`).classList.add('swap');
            });

            setTimeout(() => {
                window.location.href = loginBtn.href;
            }, 800);
        });
    }
});
/*
    RECOMENDAÇÕES DO CHEFE
*/

document.addEventListener("DOMContentLoaded", () => {
    const wrapper = document.querySelector(".carrossel-wrapper  ");
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

    // mostra o primeiro
    showSlide(index);

    // troca a cada 15 segundos
    setInterval(nextSlide, 15000);
});

/*
    CARRINHO
*/

function FinalizarCompra(username, idPedido) {

    axios.get('/api/usuarios/listar').then(response => {
        console.log(username);

        const usuarioExistente = response.data.find(p => p.email == username);

        if (usuarioExistente) {

            if (document.getElementById("cepInput").value != '' && (usuarioExistente.cep != null || usuarioExistente.cep == "")) {

                const dados = {

                    Cep: document.getElementById("cepInput").value

                }

                axios.patch(`/api/usuarios/update/${usuarioExistente.idUsuario}`, dados);

                const novosDados = {
                    estado: "Cozinhando",
                };

                axios.patch(`/api/pedidos/update/${idPedido}`, novosDados)
                    .then(res => {
                        // Redirecionar para página de confirmação
                        window.location.href = `/Home/OrderConfirmation/${idPedido}`;
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
                        // Redirecionar para página de confirmação
                        window.location.href = `/Home/OrderConfirmation/${idPedido}`;
                    })
                    .catch(err => console.error('Erro ao atualizar valor:', err));
            }
        }
    });
}

function FecharPopUp(){
    document.getElementById("PopUpNaoTemCEP").classList.remove("Aparecer");
}

/*
    HAMBURGUERES DO CARDAPIO
*/

document.addEventListener("DOMContentLoaded", () => {
    const SUPABASE_URL = "https://qspldknkkndxhlvsrbbl.supabase.co";
    const SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFzcGxka25ra25keGhsdnNyYmJsIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTU4NzEyMTAsImV4cCI6MjA3MTQ0NzIxMH0.MSQyhPRtNzetoM08Zgbq5-UQiQKUiAp3Uo_1qR5i6l4";
    const STORAGE_BUCKET = "hamburgueres";

    const fileInput = document.getElementById('fileInput');
    const linkInput = document.getElementById('LinkDaImagem');
    const imgPreview = document.getElementById('ImagemDoLink');
    const form = document.querySelector('form');
    const uploadLabel = document.querySelector('.UploadLabel');

    let selectedFile = null;
    let imageURL = "";

    linkInput.addEventListener('input', () => {
        const url = linkInput.value.trim();

        if (url.startsWith('http')) {
            imgPreview.src = url;
            imageURL = url;
            selectedFile = null;
        }
    });

    fileInput.addEventListener('change', e => {
        const file = e.target.files[0];

        if (file) {
            selectedFile = file;
            imageURL = "";
            imgPreview.src = URL.createObjectURL(file);

            linkInput.disabled = true;
            linkInput.value = file.name;

            uploadLabel.innerHTML = '<i class="fa-solid fa-xmark"></i>';
            uploadLabel.title = "Remover arquivo selecionado";
        } else {
            selectedFile = null;
            linkInput.disabled = false;
            linkInput.value = "";
        }

    });

    uploadLabel.addEventListener('click', e => {
        if (selectedFile) {
            e.preventDefault();

            selectedFile = null;
            fileInput.value = "";
            linkInput.disabled = false;
            linkInput.value = "";
            imgPreview.src = "/css/Imagens/BurguerLogo.png";

            uploadLabel.innerHTML = '<i class="fa-solid fa-folder-open"></i>';
            uploadLabel.title = "Selecionar imagem do computador";
        }
    });

    document.getElementById("BotaoSubmit").addEventListener('click', async e => {
        e.preventDefault();

        const preco = document.querySelector('input[name="Preco"]').value.trim();

        if (isNaN(preco)) {
            return;
        }

        if (selectedFile) {
            const fileName = `${Date.now()}_${selectedFile.name}`;

            const { data, error } = await supabase.storage
                .from('hamburgueres')
                .upload(fileName, selectedFile);

            if (error) {
                alert("Erro ao enviar imagem: " + error.message);
                return;
            }

            const { data: publicURL } = supabase
                .storage
                .from('hamburgueres')
                .getPublicUrl(fileName);

            imageURL = publicURL.publicUrl;
        }
        linkInput.disabled = false;
        linkInput.value = imageURL;
    });
});

$("#LinkDaImagem").blur(function () {
    const input = document.getElementById("LinkDaImagem");
    const img = document.getElementById("ImagemDoLink");

    img.src = input.value
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
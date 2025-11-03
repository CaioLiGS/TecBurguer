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
    const interfaceBox = document.querySelector(".interface");
    interfaceBox.classList.add("mostrar");
    console.log(interfaceBox.className);
}

function RemoverQuantidadeHamburguerPedido(IdPedidoHamburguer, preco){
    const url = `/api/pedidohamburgueres/update/${IdPedidoHamburguer}`;

    axios.get('/api/pedidohamburgueres/listar').then(response => {
        
        response.data.forEach(item => {
            if (item.id == IdPedidoHamburguer){
                const dados = {
                    id: IdPedidoHamburguer,
                    idPedido: item.idPedido,
                    idHamburguer: item.idHamburguer,
                    quantidade: item.quantidade - 1
                };

                if (dados.quantidade <= 0){
                    axios.delete(`/api/pedidohamburgueres/delete/${item.id}`)
                    .then(res => console.log('Hamburguer removido', res.data))
                    .catch(err => console.error('Erro ao atualizar pedido:', err));
                }
                else{
                    axios.put(`/api/pedidohamburgueres/update/${item.id}`, dados)
                    .then(res => console.log('Atualizou pedido', res.data))
                    .catch(err => console.error('Erro ao atualizar pedido:', err));
                }

                location.reload();

                axios.get('/api/pedidos/listar').then(response => {
                    const pedidoExistente = response.data.find(p => p.idPedido === item.idPedido);

                    if (pedidoExistente) {
                        const novosDados = {
                            idPedido: pedidoExistente.idPedido,
                            nome: pedidoExistente.nome,
                            precoTotal: pedidoExistente.precoTotal - preco,
                            estado: pedidoExistente.estado,
                            idUsuario: pedidoExistente.idUsuario
                        };

                        axios.put(`/api/pedidos/update/${pedidoExistente.idPedido}`, novosDados)
                            .then(res => console.log('Preço atualizado', res.data))
                            .catch(err => console.error('Erro ao atualizar valor:', err));
                    }
                });
            }
            
        });
    });
    
}

function adicionarPedidosHamburgueres(idPedido, idHamburguer) {
    const url = '/api/pedidohamburgueres/create';
    const dados = { IdPedido: idPedido, IdHamburguer: idHamburguer, quantidade: 1 };

    axios.get('/api/pedidohamburgueres/listar').then(response => {
        let criar = true;

        response.data.forEach(item => {
            if (idHamburguer == item.idHamburguer) {
                criar = false;
                dados.id = item.id;
                dados.quantidade = item.quantidade + 1;

                axios.put(`/api/pedidohamburgueres/update/${item.id}`, dados)
                    .then(res => console.log('Atualizou pedido', res.data))
                    .catch(err => console.error('Erro ao atualizar pedido:', err));
            }
        });

        if (criar) {
            axios.post(url, dados)
                .then(res => console.log('Pedido criado com sucesso:', res.data))
                .catch(err => console.error('Erro ao criar pedido:', err));
        }
    });
}

function adicionarPedidos(nome, preco, idUsuario, idHamburguer) {
    const url = '/api/pedidos/create';
    const dados = {
        nome: `Pedido_${idUsuario}${nome}`,
        PrecoTotal: preco,
        estado: "Decidindo",
        IdUsuario: idUsuario
    };

    axios.post(url, dados)
        .then(response => {
            console.log('Pedido criado', response.data);
            adicionarPedidosHamburgueres(response.data.idPedido, idHamburguer);
        })
        .catch(err => console.error('Erro ao adicionar pedido:', err));
}

function adicionarAoCarrinho(nome, preco, idHamburguer, emailUsuario) {

    axios.get('/api/usuarios/listar').then(response => {
        const usuario = response.data.find(u => u.email === emailUsuario);

        if (!usuario) return;

        const idUsuario = usuario.idUsuario;

        axios.get('/api/pedidos/listar').then(response => {
            const pedidoExistente = response.data.find(p => p.idUsuario === idUsuario);

            if (pedidoExistente) {
                const novosDados = {
                    idPedido: pedidoExistente.idPedido,
                    nome: pedidoExistente.nome,
                    precoTotal: pedidoExistente.precoTotal + preco,
                    estado: pedidoExistente.estado,
                    idUsuario: pedidoExistente.idUsuario
                };

                adicionarPedidosHamburgueres(pedidoExistente.idPedido, idHamburguer);

                axios.put(`/api/pedidos/update/${pedidoExistente.idPedido}`, novosDados)
                    .then(res => console.log('Preço atualizado', res.data))
                    .catch(err => console.error('Erro ao atualizar valor:', err));
            } else {
                adicionarPedidos(nome, preco, idUsuario, idHamburguer);
            }
        });
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

        BANNER DO HOME

*/

//document.addEventListener('DOMContentLoaded', function () {
//    const sections = [
//        document.getElementById('BannerContent'),
//        document.getElementById('OfertasDiarias'),
//        document.getElementById('RecomendacoesDoChefe'),
//        document.getElementById('Cardapio')
//    ];
//    let currentSection = 0;
//    let isScrolling = false;

//    window.addEventListener('wheel', function (e) {
//        if (isScrolling) return;

//        if (e.deltaY > 0 && currentSection < sections.length - 1) {
//            currentSection++;
//            isScrolling = true;
//            sections[currentSection].scrollIntoView({ behavior: 'smooth', block: 'center' });
//            setTimeout(() => { isScrolling = false; }, 700);
//            e.preventDefault();
//        } else if (e.deltaY < 0 && currentSection > 0) {
//            currentSection--;
//            isScrolling = true;
//            sections[currentSection].scrollIntoView({ behavior: 'smooth', block: 'center' });
//            setTimeout(() => { isScrolling = false; }, 700);
//            e.preventDefault();
//        }

//    }, { passive: false });
//});
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
function FinalizarCompra(userName, pedido){
    axios.get('/api/usuarios/listar').then(response => {
        const usuarioExistente = response.data.find(p => p.Email === userName);

        if (usuarioExistente) {
            if (usuario.Cep == null){
                document.getElementById("PopUpNaoTemCEP").classList.add("Aparecer");
            }else{
                pedido.Estado = "Cozinhando";
            }
        }
    });
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

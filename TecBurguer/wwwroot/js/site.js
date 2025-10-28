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
    console.log(IdHamburguer);

    const dados = {
        IdPedido: IdPedido,
        IdHamburguer: IdHamburguer,
        quantidade: 1
    }

    axios.get('/api/pedidohamburgueres/listar').then(function (response) {

        criar = true;

        for (var i = 0; i < response.data.length; i++) {
            if (IdHamburguer == response.data[i].idHamburguer) {

                criar = false;

                dados.id = response.data[i].id;
                dados.quantidade = response.data[i].quantidade + 1;

                axios.put(`/api/pedidohamburgueres/update/${response.data[i].id}`, dados).then(response => {
                    console.log('Criou pedidos', response.data);
                    
                }).catch(error => {
                    console.error('Erro ao adicionar pedido:', error);
                });

                break;

            }
        }

        if (criar) {
            axios.post(url, dados)
            .then(response => {
                console.log('Sucesso! Resposta do servidor:', response.data);
            })
            .catch(error => {
                console.error('Erro ao adicionar pedido:', error);
            });
        }
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
                                precoTotal: precoAnterior + preco,
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

        form.submit();
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
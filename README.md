# 🍔 TecBurguer - Sistema de Gerenciamento de Hamburgueria

<p align="center">
  <img src="https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=for-the-badge" alt="Status">
  <img src="https://img.shields.io/badge/Deploy-Railway-0b0d0e?style=for-the-badge&logo=railway" alt="Railway">
</p>

> **Acesse o projeto:** [tecburguer-production.up.railway.app](https://tecburguer.onrender.com)

O **TecBurguer** é uma solução completa para a gestão de uma hamburgueria, abrangendo desde a interface de compra do cliente até o controle administrativo e operacional (Cozinha e Entrega). Desenvolvido com foco em organização e eficiência utilizando a arquitetura MVC.

---

## 🛠 Tecnologias e Ferramentas

O projeto foi construído utilizando as seguintes linguagens e tecnologias:

| Categoria | Tecnologias |
| :--- | :--- |
| **Backend** | ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) ![ASP.NET MVC](https://img.shields.io/badge/ASP.NET%20MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) |
| **Frontend** | ![HTML5](https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white) ![CSS3](https://img.shields.io/badge/css3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white) ![JavaScript](https://img.shields.io/badge/javascript-%23F7DF1E.svg?style=for-the-badge&logo=javascript&logoColor=black) |
| **Banco de Dados** | ![Supabase](https://img.shields.io/badge/Supabase-3ECF8E?style=for-the-badge&logo=supabase&logoColor=white) (PostgreSQL via Npgsql) |
| **Hospedagem** | ![Railway](https://img.shields.io/badge/Railway-0B0D0E?style=for-the-badge&logo=railway&logoColor=white) |
| **APIs Externas** | [ViaCEP](https://viacep.com.br/) (Integração de Endereço) |

### Composição do Código
- **HTML:** 41.7%
- **C#:** 30.0%
- **CSS:** 23.5%
- **JavaScript:** 4.8%

---

## 🚀 Funcionalidades

O sistema é dividido em quatro frentes principais para atender a todos os stakeholders do negócio:

### 🛒 Loja (Cliente)
- Interface intuitiva para visualização do cardápio.
- Carrinho de compras e finalização de pedidos.
- Integração com API BuscaCEP para agilizar o cadastro de entrega.

### 🛡️ Painel Administrativo
- **Estoque:** Controle total de insumos e produtos.
- **Financeiro:** Gestão de lucros e relatórios de vendas.
- **RH:** Gerenciamento de funcionários e permissões.

### 👨‍🍳 Módulo do Cozinheiro
- Visualização de pedidos em tempo real.
- Alteração de status do pedido (Em preparo / Pronto).

### 🛵 Módulo do Entregador
- Listagem de pedidos prontos para entrega.
- Informações de endereço integradas para logística.

---
## 📸 Demonstração

### Tela inicial
<p align="center">
  <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/4840f4d8-c515-410a-ae17-9c06f1d82c95" />
</p>

### Loja do sistema
<p align="center">
  <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/58425b96-7f1e-4a75-8cd0-b7121fc5166e" />
</p>

### Administração de hamburgueres
<p align="center">
  <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/28f45c57-f359-4af1-a2a0-b9a2ddc6678f" />
</p>

---

## 🏗 Arquitetura do Sistema

O projeto utiliza o padrão **MVC (Model-View-Controller)**, garantindo a separação de responsabilidades e facilitando a manutenção do código.


<img width="865" height="533" alt="supabase-schema-qspldknkkndxhlvsrbbl" src="https://github.com/user-attachments/assets/5174ec30-bd21-4990-a9c0-45149ec72808" />


1.  **Model:** Gerencia os dados e a lógica de negócios, conectando-se ao **Supabase**.
2.  **View:** Interface do usuário (HTML/CSS).
3.  **Controller:** Intermediário que processa as requisições e atualiza as Views.

---

## ⚙️ Como executar o projeto localmente

1. **Clone o repositório:**
   ```bash
   git clone [https://github.com/CaioLiGS/TecBurguer.git](https://github.com/CaioLiGS/TecBurguer.git)

2. **Execute a aplicação:** Abra o projeto no Visual Studio ou via terminal:
   ```bash
   dotnet run

---
📄 Licença
Este projeto está sob a licença MIT.

Desenvolvido por CaioLiGS.

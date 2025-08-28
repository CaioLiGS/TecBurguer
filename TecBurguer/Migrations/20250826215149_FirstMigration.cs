using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TecBurguer.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    IdAdministrador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Nome = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__2B3E34A8112FAF2D", x => x.IdAdministrador);
                });

            migrationBuilder.CreateTable(
                name: "Hamburguer",
                columns: table => new
                {
                    IdHamburguer = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(350)", unicode: false, maxLength: 350, nullable: true),
                    Preco = table.Column<decimal>(type: "numeric(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Hamburgu__5BD4D479C15C344C", x => x.IdHamburguer);
                });

            migrationBuilder.CreateTable(
                name: "Ingrediente",
                columns: table => new
                {
                    IdIngrediente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ingredie__3DA4DD60DD5CC092", x => x.IdIngrediente);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__5B65BF9793CB8078", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "HamburguerIgrediente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdHamburguer = table.Column<int>(type: "integer", nullable: true),
                    IdIngrediente = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeNecessario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HamburguerIgrediente", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Hamburgue__IdHam__5070F446",
                        column: x => x.IdHamburguer,
                        principalTable: "Hamburguer",
                        principalColumn: "IdHamburguer");
                    table.ForeignKey(
                        name: "FK__Hamburgue__IdIng__4F7CD00D",
                        column: x => x.IdIngrediente,
                        principalTable: "Ingrediente",
                        principalColumn: "IdIngrediente");
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(350)", unicode: false, maxLength: 350, nullable: true),
                    DataHorario = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    PrecoTotal = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    IdHamburguer = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedido__9D335DC34C60A6BC", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK__Pedido__IdHambur__5441852A",
                        column: x => x.IdHamburguer,
                        principalTable: "Hamburguer",
                        principalColumn: "IdHamburguer");
                    table.ForeignKey(
                        name: "FK__Pedido__IdUsuari__534D60F1",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Entregador",
                columns: table => new
                {
                    IdEntregador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    Avaliacao = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Veiculo = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    IdPedido = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Entregad__F403B4FC8497C2EE", x => x.IdEntregador);
                    table.ForeignKey(
                        name: "FK__Entregado__IdPed__571DF1D5",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "IdPedido");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entregador_IdPedido",
                table: "Entregador",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_HamburguerIgrediente_IdHamburguer",
                table: "HamburguerIgrediente",
                column: "IdHamburguer");

            migrationBuilder.CreateIndex(
                name: "IX_HamburguerIgrediente_IdIngrediente",
                table: "HamburguerIgrediente",
                column: "IdIngrediente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdHamburguer",
                table: "Pedido",
                column: "IdHamburguer");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdUsuario",
                table: "Pedido",
                column: "IdUsuario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Entregador");

            migrationBuilder.DropTable(
                name: "HamburguerIgrediente");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Ingrediente");

            migrationBuilder.DropTable(
                name: "Hamburguer");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}

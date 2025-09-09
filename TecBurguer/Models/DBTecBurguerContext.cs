    using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TecBurguer.Models
{
    public partial class DBTecBurguerContext : DbContext
    {
        public DBTecBurguerContext()
        {
        }

        public DBTecBurguerContext(DbContextOptions<DBTecBurguerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrador> Administradors { get; set; } = null!;
        public virtual DbSet<Entregador> Entregadors { get; set; } = null!;
        public virtual DbSet<Hamburguer> Hamburguers { get; set; } = null!;
        public virtual DbSet<HamburguerIgrediente> HamburguerIgredientes { get; set; } = null!;
        public virtual DbSet<Ingrediente> Ingredientes { get; set; } = null!;
        public virtual DbSet<Pedido> Pedidos { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("User Id=postgres.qspldknkkndxhlvsrbbl;Password=Burguer@0410;Server=aws-1-sa-east-1.pooler.supabase.com;Port=5432;Database=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.IdAdministrador)
                    .HasName("PK__Administ__2B3E34A8112FAF2D");

                entity.ToTable("Administrador");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Entregador>(entity =>
            {
                entity.HasKey(e => e.IdEntregador)
                    .HasName("PK__Entregad__F403B4FC8497C2EE");

                entity.ToTable("Entregador");

                entity.Property(e => e.Avaliacao).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Veiculo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPedidoNavigation)
                    .WithMany(p => p.Entregadors)
                    .HasForeignKey(d => d.IdPedido)
                    .HasConstraintName("FK__Entregado__IdPed__571DF1D5");
            });

            modelBuilder.Entity<Hamburguer>(entity =>
            {
                entity.HasKey(e => e.IdHamburguer)
                    .HasName("PK__Hamburgu__5BD4D479C15C344C");

                entity.ToTable("Hamburguer");

                entity.Property(e => e.Descricao)
                    .HasMaxLength(350)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Imagem)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Preco).HasColumnType("numeric(18, 0)");
            });

            modelBuilder.Entity<HamburguerIgrediente>(entity =>
            {
                entity.ToTable("HamburguerIgrediente");

                entity.HasOne(d => d.IdHamburguerNavigation)
                    .WithMany(p => p.HamburguerIgredientes)
                    .HasForeignKey(d => d.IdHamburguer)
                    .HasConstraintName("FK__Hamburgue__IdHam__5070F446");

                entity.HasOne(d => d.IdIngredienteNavigation)
                    .WithMany(p => p.HamburguerIgredientes)
                    .HasForeignKey(d => d.IdIngrediente)
                    .HasConstraintName("FK__Hamburgue__IdIng__4F7CD00D");
            });

            modelBuilder.Entity<Ingrediente>(entity =>
            {
                entity.HasKey(e => e.IdIngrediente)
                    .HasName("PK__Ingredie__3DA4DD60DD5CC092");

                entity.ToTable("Ingrediente");

                entity.Property(e => e.Nome)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.IdPedido)
                    .HasName("PK__Pedido__9D335DC34C60A6BC");

                entity.ToTable("Pedido");

                entity.Property(e => e.DataHorario)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Descricao)
                    .HasMaxLength(350)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PrecoTotal).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.IdHamburguerNavigation)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.IdHamburguer)
                    .HasConstraintName("FK__Pedido__IdHambur__5441852A");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK__Pedido__IdUsuari__534D60F1");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PK__Usuario__5B65BF9793CB8078");

                entity.ToTable("Usuario");

                entity.Property(e => e.Cep)
                    .HasMaxLength(50)
                    .HasDefaultValue(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(100)
                    .HasDefaultValue(false);
                entity.Property(e => e.Cidade)
                    .HasMaxLength(100)
                    .HasDefaultValue(false);
                entity.Property(e => e.Bairro)
                    .HasMaxLength(100)
                    .HasDefaultValue(false);
                entity.Property(e => e.Rua)
                    .HasMaxLength(100)
                    .HasDefaultValue(false);
                entity.Property(e => e.Administrador)
                    .IsUnicode(false)
                    .HasDefaultValue(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

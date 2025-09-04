using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecBurguer.Areas.Identity.Data;
using TecBurguer.Models;

namespace TecBurguer.Areas.Identity.Data;

public class LoginContext : IdentityDbContext<LoginCliente>
{
    public LoginContext(DbContextOptions<LoginContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfig());

    }

    public class UserConfig : IEntityTypeConfiguration<LoginCliente>
    {
        public void Configure(EntityTypeBuilder<LoginCliente> builder)
        {
            builder.Property(x => x.Nome).HasMaxLength(100);
            builder.Property(x => x.Administrador).HasDefaultValue(false);
        }
    }
}

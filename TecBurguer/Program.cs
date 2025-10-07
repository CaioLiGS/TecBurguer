using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TecBurguer.Areas.Identity.Data;
using TecBurguer.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBTecBurguerContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<LoginContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDefaultIdentity<LoginCliente>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LoginContext>();

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Administrador", "Vendedor", "Usuário", "Entregador" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.MapRazorPages();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ----------------------
// ✅ Opção 1: Cultura pt-BR
// ----------------------
var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);

// Força todas as threads a usarem pt-BR
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
// ----------------------

app.Run();

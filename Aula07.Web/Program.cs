using System.Globalization;
using Aula07.Web.Data;
using Aula07.Web.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(options =>
{
    // Protege todas as páginas da pasta /Produtos (exige autenticação)
    options.Conventions.AuthorizeFolder("/Produtos");
    // Permite acesso anônimo às páginas de /Account (Login, Register, etc.)
    options.Conventions.AllowAnonymousToFolder("/Account");

    options.Conventions.AddFolderApplicationModelConvention(
        "/Produtos",
        model => model.Filters.Add(new PageAccessLogFilter()));
});

// DbContext com PostgreSQL e schema 'aula07'
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configurações amigáveis de senha para testes da aula
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configuração do Cookie de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AcessoNegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddSession();

var app = builder.Build();

// Migrations automáticas e Seed no startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Cultura fixa em pt-BR (garante formatação monetária e separador decimal)
var ptBrCulture = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBrCulture),
    SupportedCultures = [ptBrCulture],
    SupportedUICultures = [ptBrCulture]
});

app.UseStaticFiles();

app.UseRouting();

// UseSession deve vir após UseRouting e antes de UseAuthentication/UseAuthorization
app.UseSession();

// Autenticação DEVE vir antes da Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

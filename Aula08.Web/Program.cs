using Hipermidia.Data;
using Aula08.Web;
using Aula08.Web.Data;
using Aula08.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext com PostgreSQL e schema 'aula08'
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ProdutoService
builder.Services.AddScoped<ProdutoService>();

var app = builder.Build();

// Migrations automáticas e Seed no startup
// Schema "hipermidia" (migrations ficam em Hipermidia.Data)
HipermidiaMigrator.Aplicar(builder.Configuration.GetConnectionString("DefaultConnection"));

using (var context = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
{
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

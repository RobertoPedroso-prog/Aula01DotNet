using Aula09.Web;
using Aula09.Web.Data;
using Aula09.Web.Services;
using Hipermidia.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext com PostgreSQL e schema 'hipermidia' (compartilhado por todas as aulas)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ProdutoService + notificador de alterações (singleton, compartilhado entre circuitos)
builder.Services.AddSingleton<ProdutoEventos>();
builder.Services.AddScoped<ProdutoService>();

// TemaService
builder.Services.AddScoped<TemaService>();

var app = builder.Build();

// Schema 'hipermidia' (migrations em Hipermidia.Data) e Seed no startup
HipermidiaMigrator.Aplicar(connectionString);
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

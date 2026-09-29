using Aula09.Web;
using Aula09.Web.Data;
using Aula09.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext com PostgreSQL e schema 'aula09'
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ProdutoService
builder.Services.AddScoped<ProdutoService>();

// TemaService
builder.Services.AddScoped<TemaService>();

var app = builder.Build();

// Migrations automáticas e Seed no startup
using (var context = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
{
    context.Database.Migrate();
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

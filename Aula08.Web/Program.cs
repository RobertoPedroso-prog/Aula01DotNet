using Aula08.Web;
using Aula08.Web.Data;
using Aula08.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext com PostgreSQL e schema 'aula08'
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ProdutoService
builder.Services.AddScoped<ProdutoService>();

var app = builder.Build();

// Migrations automáticas e Seed no startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

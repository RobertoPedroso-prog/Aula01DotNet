using Aula01.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container de injeção de dependência
builder.Services.AddControllersWithViews();

// Registro do serviço criado
builder.Services.AddScoped<MensagemService>();

var app = builder.Build();

// Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
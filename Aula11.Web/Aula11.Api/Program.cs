using Aula11.Api.Data;
using Aula11.Api.Services;
using Hipermidia.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");

// Schema "hipermidia" compartilhado por todas as aulas (migrations ficam em Hipermidia.Data)
HipermidiaMigrator.Aplicar(connectionString);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<ProdutoService>();

// Ambiente local: o Blazor WASM roda em outra porta de localhost (dev server ou publicado)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWasm", policy => policy
        .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseCors("AllowWasm");
app.MapControllers();

app.Run();

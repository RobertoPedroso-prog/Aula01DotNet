using Aula10.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost:54322;Database=postgres;Username=postgres;Password=postgres;SearchPath=aula10";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWasm", policy =>
    {
        policy.WithOrigins("https://localhost:5002")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowWasm");
app.MapControllers();

app.Run();

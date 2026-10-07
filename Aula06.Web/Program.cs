using Hipermidia.Data;
using System.Globalization;
using Aula06.Web.Data;
using Aula06.Web.Filters;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddFolderApplicationModelConvention(
        "/Produtos",
        model => model.Filters.Add(new PageAccessLogFilter()));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();

var app = builder.Build();

// Schema "hipermidia" (migrations ficam em Hipermidia.Data)
HipermidiaMigrator.Aplicar(builder.Configuration.GetConnectionString("DefaultConnection"));

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Cultura fixa em pt-BR: garante que "349,90" seja interpretado como decimal
// (349.90) tanto no binding quanto na exibição, independente da cultura do SO.
var ptBrCulture = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBrCulture),
    SupportedCultures = [ptBrCulture],
    SupportedUICultures = [ptBrCulture]
});

app.UseRouting();

// Precisa vir depois de UseRouting e antes de UseAuthorization/endpoints
// para que Session esteja disponível nas Razor Pages e no PageAccessLogFilter.
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();

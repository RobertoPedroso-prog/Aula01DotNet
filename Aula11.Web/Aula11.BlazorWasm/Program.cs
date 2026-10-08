using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Aula11.BlazorWasm;
using Aula11.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Endereço da API vem de wwwroot/appsettings.json (padrão: API local da Aula 11)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5234/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// As páginas nunca usam HttpClient direto: falam com a API pelo cliente injetado
builder.Services.AddScoped<ProdutosApiClient>();

await builder.Build().RunAsync();

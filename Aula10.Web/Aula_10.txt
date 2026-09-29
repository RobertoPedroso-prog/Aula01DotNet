dotnet new webapi -n Aula10.Api
cd Aula10.Api
dotnet run

_

using Microsoft.AspNetCore.Mvc;

namespace Aula10.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var produtos = new[]
            {
                new { Id = 1, Nome = "Notebook", Preco = 4500 },
                new { Id = 2, Nome = "Mouse", Preco = 150 }
            };

            return Ok(produtos);
        }
    }
}


_

https://localhost:5001/api/produtos


_

dotnet new blazorwasm -n Aula10.BlazorWasm --pwa
cd Aula10.BlazorWasm
dotnet run


_

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/")
    });


_

namespace Aula10.BlazorWasm.Models
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}

_

@page "/produtos"
@using Aula10.BlazorWasm.Models
@inject HttpClient Http

<h3>Produtos</h3>

@if (produtos == null)
{
    <p>Carregando...</p>
}
else
{
    <ul>
    @foreach (var p in produtos)
    {
        <li>@p.Nome - @p.Preco.ToString("C")</li>
    }
    </ul>
}

@code {
    List<ProdutoDto>? produtos;

    protected override async Task OnInitializedAsync()
    {
        produtos = await Http.GetFromJsonAsync<List<ProdutoDto>>(
            "api/produtos");
    }
}

_

{
  "name": "Blazor WASM App",
  "short_name": "BlazorWASM",
  "start_url": "/",
  "display": "standalone"
}

_

dotnet publish -c Release

_

bin/Release/net8.0/publish/wwwroot

_

dotnet tool install --global dotnet-serve
dotnet serve -d wwwroot


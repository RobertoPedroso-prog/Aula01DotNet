dotnet new webapi -n Aula11.Api
cd Aula11.Api

_

namespace Aula11.Api.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; }
    }
}

_

namespace Aula11.Api.DTOs
{
    public record ProdutoDto(int Id, string Nome, decimal Preco);
}

_

namespace Aula11.Api.DTOs
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}

_

using Aula11.Api.DTOs;
using Aula11.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aula11.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private static List<Produto> _produtos = new()
        {
            new Produto { Id = 1, Nome = "Notebook", Preco = 4500, Ativo = true },
            new Produto { Id = 2, Nome = "Mouse", Preco = 150, Ativo = true },
            new Produto { Id = 3, Nome = "Teclado", Preco = 300, Ativo = false }
        };

        // GET api/v1/produtos?page=1&pageSize=2&ativo=true&nome=note
        [HttpGet]
        public IActionResult Get(
            int page = 1,
            int pageSize = 10,
            bool? ativo = null,
            string? nome = null)
        {
            var query = _produtos.AsQueryable();

            // Filtro por status
            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            // Filtro por nome
            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProdutoDto(p.Id, p.Nome, p.Preco))
                .ToList();

            var result = new PagedResult<ProdutoDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                Items = items
            };

            return Ok(result);
        }
    }
}

_

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<T> Items { get; set; }
}

_

public record ProdutoDto(int Id, string Nome, decimal Preco);

_

@page "/produtos"
@inject HttpClient Http

<h3>Produtos</h3>

<input placeholder="Nome" @bind="filtroNome" />
<select @bind="filtroAtivo">
    <option value="">Todos</option>
    <option value="true">Ativos</option>
    <option value="false">Inativos</option>
</select>

<button @onclick="Buscar">Buscar</button>

@if (resultado == null)
{
    <p>Carregando...</p>
}
else
{
    <ul>
    @foreach (var p in resultado.Items)
    {
        <li>@p.Nome - @p.Preco.ToString("C")</li>
    }
    </ul>

    <button @onclick="Anterior" disabled="@(page == 1)">Anterior</button>
    <button @onclick="Proxima">Próxima</button>
}

@code {
    PagedResult<ProdutoDto>? resultado;
    int page = 1;
    int pageSize = 2;
    string? filtroNome;
    bool? filtroAtivo;

    protected override async Task OnInitializedAsync()
    {
        await Buscar();
    }

    async Task Buscar()
    {
        var url = $"api/v1/produtos?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(filtroNome))
            url += $"&nome={filtroNome}";

        if (filtroAtivo.HasValue)
            url += $"&ativo={filtroAtivo.Value}";

        resultado = await Http.GetFromJsonAsync<PagedResult<ProdutoDto>>(url);
    }

    async Task Proxima()
    {
        page++;
        await Buscar();
    }

    async Task Anterior()
    {
        page--;
        await Buscar();
    }
}

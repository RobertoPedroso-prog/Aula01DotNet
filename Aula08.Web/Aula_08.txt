dotnet new blazorserver -n Aula08.BlazorServer
cd Aula08.BlazorServer
dotnet run

_

namespace Aula08.BlazorServer.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}


_

using Aula08.BlazorServer.Models;

namespace Aula08.BlazorServer.Services
{
    public class ProdutoService
    {
        private List<Produto> _produtos = new();
        private int _id = 1;

        public List<Produto> ObterTodos() => _produtos;

        public void Adicionar(Produto produto)
        {
            produto.Id = _id++;
            _produtos.Add(produto);
        }

        public void Remover(int id)
        {
            var produto = _produtos.First(p => p.Id == id);
            _produtos.Remove(produto);
        }
    }
}

_

builder.Services.AddScoped<ProdutoService>();


_

@page "/produtos"
@using Aula08.BlazorServer.Models
@inject ProdutoService ProdutoService

<h3>Produtos</h3>

<ProdutoForm OnSalvar="AdicionarProduto" />

<ProdutoList Produtos="Produtos" OnExcluir="ExcluirProduto" />

@code {
    private List<Produto> Produtos = new();

    protected override void OnInitialized()
    {
        Produtos = ProdutoService.ObterTodos();
    }

    void AdicionarProduto(Produto produto)
    {
        ProdutoService.Adicionar(produto);
        StateHasChanged();
    }

    void ExcluirProduto(int id)
    {
        ProdutoService.Remover(id);
        StateHasChanged();
    }
}

_

@using Aula08.BlazorServer.Models

<EditForm Model="Produto" OnValidSubmit="Salvar">
    <InputText @bind-Value="Produto.Nome" placeholder="Nome" />
    <InputNumber @bind-Value="Produto.Preco" placeholder="Preço" />
    <button type="submit">Salvar</button>
</EditForm>

@code {
    private Produto Produto = new();

    [Parameter]
    public EventCallback<Produto> OnSalvar { get; set; }

    async Task Salvar()
    {
        await OnSalvar.InvokeAsync(Produto);
        Produto = new Produto(); // reset
    }
}

_

@using Aula08.BlazorServer.Models

<table>
@foreach (var p in Produtos)
{
    <tr>
        <td>@p.Nome</td>
        <td>@p.Preco</td>
        <td>
            <button @onclick="() => OnExcluir.InvokeAsync(p.Id)">
                Excluir
            </button>
        </td>
    </tr>
}
</table>

@code {
    [Parameter]
    public List<Produto> Produtos { get; set; }

    [Parameter]
    public EventCallback<int> OnExcluir { get; set; }
}

_


protected override void OnInitialized()
{
    // Carrega dados iniciais
}


_

StateHasChanged(); // força renderização



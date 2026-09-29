<div class="card">
    <h4>@Titulo</h4>
    <div class="card-body">
        @ChildContent
    </div>
</div>

@code {
    [Parameter]
    public string Titulo { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }
}

_

<Card Titulo="Resumo">
    <p>Total de Produtos: @Total</p>
</Card>

_

using System.ComponentModel.DataAnnotations;

public class Produto
{
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    public string Nome { get; set; }

    [Range(0.01, 99999)]
    public decimal Preco { get; set; }

    public int Estoque { get; set; }
}

_

@using Microsoft.AspNetCore.Components.Forms

<EditForm EditContext="editContext" OnValidSubmit="Salvar">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <InputText @bind-Value="Produto.Nome" placeholder="Nome" />
    <InputNumber @bind-Value="Produto.Preco" placeholder="Preço" />
    <InputNumber @bind-Value="Produto.Estoque" placeholder="Estoque" />

    <button type="submit">Salvar</button>
</EditForm>

@code {
    [Parameter]
    public Produto Produto { get; set; } = new();

    [Parameter]
    public EventCallback<Produto> OnSalvar { get; set; }

    private EditContext editContext;
    private ValidationMessageStore messages;

    protected override void OnInitialized()
    {
        editContext = new EditContext(Produto);
        messages = new ValidationMessageStore(editContext);

        editContext.OnValidationRequested += CustomValidation;
    }

    void CustomValidation(object sender, ValidationRequestedEventArgs e)
    {
        messages.Clear();

        if (Produto.Estoque < 0)
        {
            messages.Add(
                () => Produto.Estoque,
                "Estoque não pode ser negativo");
        }
    }

    async Task Salvar()
    {
        await OnSalvar.InvokeAsync(Produto);
    }
}

_


@page "/dashboard"
@inject ProdutoService ProdutoService

<h3>Dashboard</h3>

<div class="dashboard">
    <Card Titulo="Total de Produtos">
        <h2>@TotalProdutos</h2>
    </Card>

    <Card Titulo="Valor Médio">
        <h2>@PrecoMedio.ToString("C")</h2>
    </Card>
</div>

@code {
    int TotalProdutos;
    decimal PrecoMedio;

    protected override void OnInitialized()
    {
        var produtos = ProdutoService.ObterTodos();

        TotalProdutos = produtos.Count;
        PrecoMedio = produtos.Any()
            ? produtos.Average(p => p.Preco)
            : 0;
    }
}

_


using Aula03.Web.Interfaces;
using Aula03.Web.Models;

namespace Aula03.Web.Services;

public class ProdutoService : IProdutoService
{
    // static para persistir entre requisições, já que o serviço é registrado como Scoped
    private static readonly List<Produto> _produtos = new()
    {
        new Produto { Id = 1, Nome = "Teclado", Categoria = "Periféricos", Preco = 150.00m, Ativo = true },
        new Produto { Id = 2, Nome = "Monitor", Categoria = "Periféricos", Preco = 899.90m, Ativo = true },
        new Produto { Id = 3, Nome = "Mouse", Categoria = "Periféricos", Preco = 79.90m, Ativo = true }
    };

    private static int _proximoId = 4;

    public async Task<List<Produto>> ObterTodosAsync()
    {
        await Task.Delay(100);
        return _produtos.OrderBy(p => p.Nome).ToList();
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        await Task.Delay(100);
        return _produtos.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Produto> CriarAsync(Produto produto)
    {
        await Task.Delay(100);
        produto.Id = _proximoId++;
        _produtos.Add(produto);
        return produto;
    }

    public async Task<bool> AtualizarAsync(Produto produto)
    {
        await Task.Delay(100);
        var existente = _produtos.FirstOrDefault(p => p.Id == produto.Id);
        if (existente is null)
            return false;

        existente.Nome = produto.Nome;
        existente.Categoria = produto.Categoria;
        existente.Preco = produto.Preco;
        existente.Ativo = produto.Ativo;
        return true;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        await Task.Delay(100);
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        if (produto is null)
            return false;

        _produtos.Remove(produto);
        return true;
    }
}

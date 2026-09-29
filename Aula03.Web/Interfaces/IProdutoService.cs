using Aula03.Web.Models;

namespace Aula03.Web.Interfaces;

public interface IProdutoService
{
    Task<List<Produto>> ObterTodosAsync();
    Task<Produto?> ObterPorIdAsync(int id);
    Task<Produto> CriarAsync(Produto produto);
    Task<bool> AtualizarAsync(Produto produto);
    Task<bool> ExcluirAsync(int id);
}

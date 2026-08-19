using Aula02.Web.DTOs;

namespace Aula02.Web.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoDto>> ObterProdutosAtivosAsync();
}

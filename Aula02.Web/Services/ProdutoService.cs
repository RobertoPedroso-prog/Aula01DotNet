using Aula02.Web.DTOs;
using Aula02.Web.Models;
using Aula02.Web.Interfaces;

namespace Aula02.Web.Services
{
    public class ProdutoService : IProdutoService
    {
        // Simulando base de dados em memória
        private readonly List<Produto> _produtos = new()
        {
            new Produto { Id = 1, Nome = "Teclado", Preco = 150.00m, Ativo = true },
            new Produto { Id = 2, Nome = "Monitor", Preco = 899.90m, Ativo = false },
            new Produto { Id = 3, Nome = "Mouse", Preco = 79.90m, Ativo = true }
        };

        public async Task<List<ProdutoDto>> ObterProdutosAtivosAsync()
        {
            // Simula operação I/O
            await Task.Delay(300);

            // LINQ avançado + projeção para DTO
            return _produtos
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoDto(p.Id, p.Nome, p.Preco))
                .ToList();
        }
    }
}

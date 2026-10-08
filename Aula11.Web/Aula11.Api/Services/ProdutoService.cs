using Aula11.Api.Data;
using Aula11.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Aula11.Api.Services;

public record ResultadoCriacao(Produto? Produto, string? Erro);

/// <summary>Regras de consulta e criação compartilhadas pelos controllers v1 e v2.</summary>
public class ProdutoService
{
    public const int PageSizeMaximo = 100;

    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public static string? ValidarPaginacao(int page, int pageSize)
    {
        if (page < 1)
            return "page deve ser maior ou igual a 1.";
        if (pageSize < 1 || pageSize > PageSizeMaximo)
            return $"pageSize deve estar entre 1 e {PageSizeMaximo}.";
        return null;
    }

    /// <summary>Filtra por status e por nome (sem diferenciar maiúsculas) e pagina no banco.</summary>
    public async Task<(List<Produto> Itens, int Total)> ListarAsync(int page, int pageSize, bool? ativo, string? nome)
    {
        var query = _context.Produtos.Include(p => p.Categoria).AsNoTracking().AsQueryable();

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            query = query.Where(p => p.Nome.ToLower().Contains(termo));
        }

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (itens, total);
    }

    public Task<Produto?> ObterAsync(int id) =>
        _context.Produtos.Include(p => p.Categoria).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<ResultadoCriacao> CriarAsync(string nome, decimal preco, bool ativo, int estoque, int? categoriaId)
    {
        var idCategoria = categoriaId
            ?? await _context.Categorias.OrderBy(c => c.Id).Select(c => (int?)c.Id).FirstOrDefaultAsync();

        if (idCategoria is null || !await _context.Categorias.AnyAsync(c => c.Id == idCategoria))
            return new ResultadoCriacao(null, "Categoria inexistente.");

        var produto = new Produto
        {
            Nome = nome.Trim(),
            Preco = preco,
            Ativo = ativo,
            Estoque = estoque,
            CategoriaId = idCategoria.Value
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();
        return new ResultadoCriacao(produto, null);
    }
}

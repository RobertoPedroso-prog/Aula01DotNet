using Aula09.Web.Data;
using Aula09.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Aula09.Web.Services;

public class ProdutoService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly ProdutoEventos _eventos;

    public ProdutoService(IDbContextFactory<AppDbContext> factory, ProdutoEventos eventos)
    {
        _factory = factory;
        _eventos = eventos;
    }

    public async Task<List<Produto>> ObterTodosAsync()
    {
        using var context = _factory.CreateDbContext();
        return await context.Produtos.Include(p => p.Categoria).ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        using var context = _factory.CreateDbContext();
        return await context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Categoria>> ObterCategoriasAsync()
    {
        using var context = _factory.CreateDbContext();
        return await context.Categorias.ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        using var context = _factory.CreateDbContext();
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        _eventos.Notificar();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        using var context = _factory.CreateDbContext();
        context.Produtos.Update(produto);
        await context.SaveChangesAsync();
        _eventos.Notificar();
    }

    public async Task RemoverAsync(int id)
    {
        using var context = _factory.CreateDbContext();
        var produto = await context.Produtos.FindAsync(id);
        if (produto != null)
        {
            context.Produtos.Remove(produto);
            await context.SaveChangesAsync();
            _eventos.Notificar();
        }
    }
}

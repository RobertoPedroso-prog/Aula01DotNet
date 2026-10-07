using Aula08.Web.Data;
using Aula08.Web.Models;
using Aula08.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Aula08.Web.Tests;

public class ProdutoServiceTests
{
    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options)
        : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }

    private static (ProdutoService Service, IDbContextFactory<AppDbContext> Factory) CriarServico()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var factory = new TestDbContextFactory(options);
        return (new ProdutoService(factory), factory);
    }

    private static async Task<Categoria> CriarCategoriaAsync(IDbContextFactory<AppDbContext> factory, string nome = "Eletrônicos")
    {
        using var context = factory.CreateDbContext();
        var categoria = new Categoria { Nome = nome };
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();
        return categoria;
    }

    private static Produto NovoProduto(int categoriaId, string nome = "Notebook", decimal preco = 4500m) =>
        new() { Nome = nome, Preco = preco, Ativo = true, CategoriaId = categoriaId };

    [Fact]
    public async Task ObterTodosAsync_SemProdutos_RetornaListaVazia()
    {
        var (service, _) = CriarServico();

        var resultado = await service.ObterTodosAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterTodosAsync_RetornaProdutosComCategoria()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        await service.AdicionarAsync(NovoProduto(categoria.Id, "Notebook"));
        await service.AdicionarAsync(NovoProduto(categoria.Id, "Mouse", 80m));

        var resultado = await service.ObterTodosAsync();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, p => Assert.Equal("Eletrônicos", p.Categoria?.Nome));
    }

    [Fact]
    public async Task ObterPorIdAsync_Existente_RetornaProdutoComCategoria()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        var produto = NovoProduto(categoria.Id);
        await service.AdicionarAsync(produto);

        var resultado = await service.ObterPorIdAsync(produto.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Notebook", resultado.Nome);
        Assert.Equal(4500m, resultado.Preco);
        Assert.Equal(categoria.Id, resultado.Categoria?.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_Inexistente_RetornaNull()
    {
        var (service, _) = CriarServico();

        var resultado = await service.ObterPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterCategoriasAsync_RetornaTodasAsCategorias()
    {
        var (service, factory) = CriarServico();
        await CriarCategoriaAsync(factory, "Eletrônicos");
        await CriarCategoriaAsync(factory, "Livros");

        var resultado = await service.ObterCategoriasAsync();

        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, c => c.Nome == "Livros");
    }

    [Fact]
    public async Task AdicionarAsync_PersisteProdutoEGeraId()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        var produto = NovoProduto(categoria.Id);

        await service.AdicionarAsync(produto);

        Assert.True(produto.Id > 0);
        using var context = factory.CreateDbContext();
        Assert.Equal(1, await context.Produtos.CountAsync());
    }

    [Fact]
    public async Task AtualizarAsync_AlteraDadosDoProduto()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        var produto = NovoProduto(categoria.Id);
        await service.AdicionarAsync(produto);

        produto.Nome = "Notebook Gamer";
        produto.Preco = 7000m;
        produto.Ativo = false;
        await service.AtualizarAsync(produto);

        var atualizado = await service.ObterPorIdAsync(produto.Id);
        Assert.NotNull(atualizado);
        Assert.Equal("Notebook Gamer", atualizado.Nome);
        Assert.Equal(7000m, atualizado.Preco);
        Assert.False(atualizado.Ativo);
    }

    [Fact]
    public async Task RemoverAsync_Existente_RemoveProduto()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        var produto = NovoProduto(categoria.Id);
        await service.AdicionarAsync(produto);

        await service.RemoverAsync(produto.Id);

        Assert.Null(await service.ObterPorIdAsync(produto.Id));
    }

    [Fact]
    public async Task RemoverAsync_Inexistente_NaoLancaExcecaoNemAfetaOutros()
    {
        var (service, factory) = CriarServico();
        var categoria = await CriarCategoriaAsync(factory);
        await service.AdicionarAsync(NovoProduto(categoria.Id));

        var excecao = await Record.ExceptionAsync(() => service.RemoverAsync(999));

        Assert.Null(excecao);
        Assert.Single(await service.ObterTodosAsync());
    }
}

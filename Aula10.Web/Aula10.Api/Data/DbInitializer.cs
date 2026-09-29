using Aula10.Api.Models;

namespace Aula10.Api.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Categorias.Any())
            return;

        var eletronicos = new Categoria { Nome = "Eletrônicos" };
        var livros = new Categoria { Nome = "Livros" };

        context.Categorias.AddRange(eletronicos, livros);
        context.SaveChanges();

        var produtos = new[]
        {
            new Produto { Nome = "Notebook", Preco = 4500, Estoque = 5, CategoriaId = eletronicos.Id },
            new Produto { Nome = "Mouse", Preco = 150, Estoque = 20, CategoriaId = eletronicos.Id },
            new Produto { Nome = "Clean Code", Preco = 120, Estoque = 10, CategoriaId = livros.Id }
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}

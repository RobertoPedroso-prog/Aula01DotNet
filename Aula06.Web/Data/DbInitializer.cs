using Aula06.Web.Models;

namespace Aula06.Web.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Categorias.Any())
            return;

        var categorias = new List<Categoria>
        {
            new() { Nome = "Eletrônicos" },
            new() { Nome = "Livros" }
        };

        context.Categorias.AddRange(categorias);
        context.SaveChanges();

        var produtos = new List<Produto>
        {
            new() { Nome = "Notebook", Preco = 4500, Ativo = true, CategoriaId = categorias[0].Id },
            new() { Nome = "Livro C#", Preco = 120, Ativo = true, CategoriaId = categorias[1].Id }
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}

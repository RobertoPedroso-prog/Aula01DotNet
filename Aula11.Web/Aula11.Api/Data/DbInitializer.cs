using Aula11.Api.Models;

namespace Aula11.Api.Data;

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

        context.Produtos.AddRange(
            new Produto { Nome = "Notebook", Preco = 4500, Ativo = true, Estoque = 5, CategoriaId = eletronicos.Id },
            new Produto { Nome = "Mouse", Preco = 150, Ativo = true, Estoque = 20, CategoriaId = eletronicos.Id },
            new Produto { Nome = "Teclado", Preco = 300, Ativo = false, Estoque = 0, CategoriaId = eletronicos.Id },
            new Produto { Nome = "Clean Code", Preco = 120, Ativo = true, Estoque = 10, CategoriaId = livros.Id });
        context.SaveChanges();
    }
}

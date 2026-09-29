namespace Aula04.Web.Models;

public class Categoria
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    // Navegação
    public List<Produto> Produtos { get; set; } = [];
}

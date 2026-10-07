namespace Hipermidia.Data.Entities;

public class Categoria
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public List<Produto> Produtos { get; set; } = [];
}

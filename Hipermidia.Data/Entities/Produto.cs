namespace Hipermidia.Data.Entities;

public class Produto
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public decimal Preco { get; set; }

    public bool Ativo { get; set; }

    // Aula 09 em diante
    public int Estoque { get; set; }

    // Aulas 06 e 07 (upload de imagem)
    public string? Imagem { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    // Aula 04 (N:N)
    public List<Pedido> Pedidos { get; set; } = [];
}

namespace Aula11.Api.Models;

/// <summary>Entidade interna (tabela hipermidia."Produtos"). Nunca é devolvida direto pela API: use os DTOs.</summary>
public class Produto
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public decimal Preco { get; set; }

    public bool Ativo { get; set; }

    public int Estoque { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}

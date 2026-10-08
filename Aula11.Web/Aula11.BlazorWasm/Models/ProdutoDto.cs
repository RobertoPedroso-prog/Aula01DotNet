namespace Aula11.BlazorWasm.Models;

/// <summary>
/// Produto como a API devolve. Id, Nome e Preco existem na v1 e na v2;
/// Ativo, Estoque e Categoria só vêm na v2 (ficam nulos quando a v1 é usada).
/// </summary>
public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }

    public bool? Ativo { get; set; }
    public int? Estoque { get; set; }
    public string? Categoria { get; set; }
}

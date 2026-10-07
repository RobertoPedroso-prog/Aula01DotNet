using System.ComponentModel.DataAnnotations;

namespace Aula09.Web.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999")]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo")]
    public int Estoque { get; set; }

    public bool Ativo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}

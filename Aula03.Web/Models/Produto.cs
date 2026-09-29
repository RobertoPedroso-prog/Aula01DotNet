using System.ComponentModel.DataAnnotations;

namespace Aula03.Web.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "A categoria deve ter entre 3 e 50 caracteres")]
    public required string Categoria { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999")]
    public decimal Preco { get; set; }

    public bool Ativo { get; set; }
}

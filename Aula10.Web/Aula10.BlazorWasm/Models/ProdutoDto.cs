using System.ComponentModel.DataAnnotations;

namespace Aula10.BlazorWasm.Models;

public class ProdutoDto
{
    public int Id { get; set; }

    // Validação no cliente (a API valida de novo: "validação dupla")
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999.")]
    public decimal Preco { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Aula11.BlazorWasm.Models;

/// <summary>Corpo do POST. Validado aqui (cliente) e de novo na API (servidor).</summary>
public class CriarProdutoRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999.")]
    public decimal Preco { get; set; }

    public bool Ativo { get; set; } = true;

    // Só é enviado/aceito pela v2
    [Range(0, 1_000_000, ErrorMessage = "O estoque deve estar entre 0 e 1.000.000.")]
    public int Estoque { get; set; }
}

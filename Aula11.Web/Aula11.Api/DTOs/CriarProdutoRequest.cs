using System.ComponentModel.DataAnnotations;

namespace Aula11.Api.DTOs;

/// <summary>Corpo do POST /api/v1/produtos. As DataAnnotations são validadas pelo [ApiController] (400 automático).</summary>
public class CriarProdutoRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999.")]
    public decimal Preco { get; set; }

    /// <summary>Opcional: se omitido, o produto entra na primeira categoria cadastrada.</summary>
    public int? CategoriaId { get; set; }

    public bool Ativo { get; set; } = true;
}

/// <summary>Corpo do POST /api/v2/produtos: a v1 mais o campo Estoque.</summary>
public class CriarProdutoV2Request : CriarProdutoRequest
{
    [Range(0, 1_000_000, ErrorMessage = "O estoque deve estar entre 0 e 1.000.000.")]
    public int Estoque { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Aula10.Api.Models;

public class Produto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
    public int Estoque { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}

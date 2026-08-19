namespace Aula02.Web.DTOs
{
    // DTO imutável usando positional record
    public record ProdutoDto(int Id, string Nome, decimal Preco);
}

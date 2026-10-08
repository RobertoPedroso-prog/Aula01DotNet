namespace Aula11.Api.DTOs;

/// <summary>Envelope de resposta paginada (usado nas duas versões da API).</summary>
public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 0;
    public List<T> Items { get; set; } = [];
}

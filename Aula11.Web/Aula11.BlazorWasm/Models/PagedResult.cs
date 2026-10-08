namespace Aula11.BlazorWasm.Models;

/// <summary>Envelope de resposta paginada devolvido pela API (v1 e v2).</summary>
public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; } = [];
}

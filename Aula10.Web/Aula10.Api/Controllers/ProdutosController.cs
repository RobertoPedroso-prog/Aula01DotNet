using System.ComponentModel.DataAnnotations;
using Aula10.Api.Data;
using Aula10.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aula10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutos()
    {
        var produtos = await _context.Produtos
            .OrderBy(p => p.Id)
            .Select(p => new ProdutoDto { Id = p.Id, Nome = p.Nome, Preco = p.Preco, CategoriaId = p.CategoriaId })
            .ToListAsync();

        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
            return NotFound();

        return Ok(new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            CategoriaId = produto.CategoriaId
        });
    }

    // Validação no servidor: [ApiController] devolve 400 automaticamente quando as DataAnnotations do DTO falham.
    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> PostProduto([FromBody] ProdutoDto dto)
    {
        var categoriaId = dto.CategoriaId
            ?? await _context.Categorias.OrderBy(c => c.Id).Select(c => (int?)c.Id).FirstOrDefaultAsync();

        if (categoriaId is null || !await _context.Categorias.AnyAsync(c => c.Id == categoriaId))
        {
            ModelState.AddModelError(nameof(dto.CategoriaId), "Categoria inexistente.");
            return ValidationProblem(ModelState);
        }

        var produto = new Produto
        {
            Nome = dto.Nome.Trim(),
            Preco = dto.Preco,
            Estoque = 0,
            CategoriaId = categoriaId.Value
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            CategoriaId = produto.CategoriaId
        });
    }
}

public class ProdutoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0.01, 99999, ErrorMessage = "O preço deve estar entre 0,01 e 99999.")]
    public decimal Preco { get; set; }

    /// <summary>Opcional: se omitido, o produto entra na primeira categoria cadastrada.</summary>
    public int? CategoriaId { get; set; }
}

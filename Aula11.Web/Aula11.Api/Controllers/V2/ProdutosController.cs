using Aula11.Api.DTOs;
using Aula11.Api.Models;
using Aula11.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aula11.Api.Controllers.V2;

/// <summary>
/// Versão 2: mesmos recursos da v1, mas o produto traz campos extras (Ativo, Estoque, Categoria)
/// e o POST aceita Estoque. A v1 não muda.
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class ProdutosController : ApiControllerBase
{
    private const string RotaPorId = "ProdutoV2PorId";

    private readonly ProdutoService _service;

    public ProdutosController(ProdutoService service)
    {
        _service = service;
    }

    // GET api/v2/produtos?page=1&pageSize=2&ativo=true&nome=note
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProdutoV2Dto>>> Get(
        int page = 1,
        int pageSize = 10,
        bool? ativo = null,
        string? nome = null)
    {
        if (PaginacaoInvalida(page, pageSize) is { } invalido)
            return invalido;

        var (itens, total) = await _service.ListarAsync(page, pageSize, ativo, nome);

        return Ok(new PagedResult<ProdutoV2Dto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            Items = itens.Select(ParaDto).ToList()
        });
    }

    // GET api/v2/produtos/5
    [HttpGet("{id:int}", Name = RotaPorId)]
    public async Task<ActionResult<ProdutoV2Dto>> GetPorId(int id)
    {
        var produto = await _service.ObterAsync(id);
        return produto is null ? NotFound() : Ok(ParaDto(produto));
    }

    // POST api/v2/produtos  -> 201 Created (ou 400 se o DTO for inválido)
    [HttpPost]
    public async Task<ActionResult<ProdutoV2Dto>> Post([FromBody] CriarProdutoV2Request request)
    {
        var resultado = await _service.CriarAsync(request.Nome, request.Preco, request.Ativo, request.Estoque, request.CategoriaId);
        if (resultado.Produto is null)
            return CategoriaInvalida(resultado.Erro!);

        return CreatedAtRoute(RotaPorId, new { id = resultado.Produto.Id }, ParaDto(resultado.Produto));
    }

    private static ProdutoV2Dto ParaDto(Produto p) => new(p.Id, p.Nome, p.Preco, p.Ativo, p.Estoque, p.Categoria?.Nome);
}

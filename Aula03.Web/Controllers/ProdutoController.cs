using Aula03.Web.Interfaces;
using Aula03.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aula03.Web.Controllers;

public class ProdutoController : Controller
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    // LISTAR
    public async Task<IActionResult> Index()
    {
        var produtos = await _produtoService.ObterTodosAsync();
        return View(produtos);
    }

    // FORMULÁRIO CRIAÇÃO
    public IActionResult Create()
    {
        return View();
    }

    // SALVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Produto produto)
    {
        if (!ModelState.IsValid)
            return View(produto);

        await _produtoService.CriarAsync(produto);
        return RedirectToAction(nameof(Index));
    }

    // FORMULÁRIO EDIÇÃO
    public async Task<IActionResult> Edit(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        if (produto is null)
            return NotFound();

        return View(produto);
    }

    // ATUALIZAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Produto produto)
    {
        if (!ModelState.IsValid)
            return View(produto);

        var atualizado = await _produtoService.AtualizarAsync(produto);
        if (!atualizado)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // EXCLUIR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _produtoService.ExcluirAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

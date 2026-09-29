using Aula04.Web.Data;
using Aula04.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aula04.Web.Controllers;

public class ProdutoController : Controller
{
    private readonly AppDbContext _context;

    public ProdutoController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR
    public async Task<IActionResult> Index()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .ToListAsync();

        return View(produtos);
    }

    // FORMULÁRIO CRIAÇÃO
    public IActionResult Create()
    {
        ViewBag.Categorias = _context.Categorias.ToList();
        return View();
    }

    // SALVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Produto produto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            return View(produto);
        }

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // FORMULÁRIO EDIÇÃO
    public async Task<IActionResult> Edit(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null)
            return NotFound();

        ViewBag.Categorias = _context.Categorias.ToList();
        return View(produto);
    }

    // ATUALIZAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Produto produto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            return View(produto);
        }

        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // EXCLUIR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is not null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}

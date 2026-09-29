using Aula04.Web.Data;
using Aula04.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aula04.Web.Controllers;

public class PedidoController : Controller
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR (pedidos com produtos)
    public async Task<IActionResult> Index()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Produtos)
            .AsNoTracking()
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        return View(pedidos);
    }

    // FORMULÁRIO CRIAÇÃO
    public IActionResult Create()
    {
        ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo).ToList();
        return View();
    }

    // SALVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DateTime dataPedido, int[] produtoIds)
    {
        if (produtoIds is null || produtoIds.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Selecione ao menos um produto.");
            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo).ToList();
            return View();
        }

        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id))
            .ToListAsync();

        var pedido = new Pedido
        {
            DataPedido = dataPedido,
            Produtos = produtos
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}

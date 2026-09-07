using System.Diagnostics;
using Aula05.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Aula05.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var promocaoDaSemana = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .OrderBy(p => p.Preco)
            .FirstOrDefaultAsync();

        return View(promocaoDaSemana);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

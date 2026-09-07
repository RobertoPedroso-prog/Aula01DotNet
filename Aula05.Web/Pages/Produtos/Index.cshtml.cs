using Aula05.Web.Data;
using Aula05.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Pages.Produtos;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    // Lista usada pela View
    public List<Produto> Produtos { get; set; } = [];

    public async Task OnGetAsync()
    {
        // Leitura sem tracking (melhor performance)
        Produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .ToListAsync();
    }
}

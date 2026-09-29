using Aula07.Web.Data;
using Aula07.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aula07.Web.Pages.Produtos;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Produto> Produtos { get; set; } = [];

    public async Task OnGetAsync()
    {
        Produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
}

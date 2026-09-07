using Aula05.Web.Data;
using Aula05.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Pages.Produtos;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Produto Produto { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        Produto = produto;
        return Page();
    }
}

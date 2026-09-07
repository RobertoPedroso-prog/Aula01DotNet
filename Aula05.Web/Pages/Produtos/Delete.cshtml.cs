using Aula05.Web.Data;
using Aula05.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Pages.Produtos;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}

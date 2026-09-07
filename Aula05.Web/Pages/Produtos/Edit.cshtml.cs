using Aula05.Web.Data;
using Aula05.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Pages.Produtos;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Produto Produto { get; set; } = null!;

    public List<SelectListItem> Categorias { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return NotFound();

        Produto = produto;
        await CarregarCategoriasAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync();
            return Page();
        }

        _context.Update(Produto);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task CarregarCategoriasAsync()
    {
        Categorias = await _context.Categorias
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome
            })
            .ToListAsync();
    }
}

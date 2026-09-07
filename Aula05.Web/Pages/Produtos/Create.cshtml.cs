using Aula05.Web.Data;
using Aula05.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Aula05.Web.Pages.Produtos;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Produto Produto { get; set; } = null!;

    public List<SelectListItem> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        Categorias = await _context.Categorias
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(); // Recarrega categorias
            return Page();
        }

        _context.Produtos.Add(Produto);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

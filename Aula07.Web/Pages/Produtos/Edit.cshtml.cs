using Aula07.Web.Data;
using Aula07.Web.Helpers;
using Aula07.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Aula07.Web.Pages.Produtos;

// Desafio 2: Restringir edição de produtos para Admin e Gerente
[Authorize(Roles = "Admin,Gerente")]
public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public EditModel(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Produto Produto { get; set; } = null!;

    [BindProperty]
    public IFormFile? ImagemUpload { get; set; }

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
        if (ImagemUpload != null)
        {
            var erro = ImagemUploadHelper.Validar(ImagemUpload);
            if (erro != null)
                ModelState.AddModelError(nameof(ImagemUpload), erro);
        }

        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync();
            return Page();
        }

        if (ImagemUpload != null)
        {
            var pastaImagens = Path.Combine(_env.WebRootPath, "imagens");
            Produto.Imagem = await ImagemUploadHelper.SalvarAsync(ImagemUpload, pastaImagens);
        }

        _context.Update(Produto);
        await _context.SaveChangesAsync();

        TempData["Mensagem"] = "Produto atualizado com sucesso!";
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

using Aula07.Web.Data;
using Aula07.Web.Helpers;
using Aula07.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Aula07.Web.Pages.Produtos;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateModel(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Produto Produto { get; set; } = null!;

    [BindProperty]
    public IFormFile? ImagemUpload { get; set; }

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
        if (ImagemUpload != null)
        {
            var erro = ImagemUploadHelper.Validar(ImagemUpload);
            if (erro != null)
                ModelState.AddModelError(nameof(ImagemUpload), erro);
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (ImagemUpload != null)
        {
            var pastaImagens = Path.Combine(_env.WebRootPath, "imagens");
            Produto.Imagem = await ImagemUploadHelper.SalvarAsync(ImagemUpload, pastaImagens);
        }

        _context.Produtos.Add(Produto);
        await _context.SaveChangesAsync();

        TempData["Mensagem"] = "Produto cadastrado com sucesso!";
        return RedirectToPage("Index");
    }
}

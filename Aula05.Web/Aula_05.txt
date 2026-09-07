using Aula01.Web.Data;
using Aula01.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Aula01.Web.Pages.Produtos
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Lista usada pela View
        public List<Produto> Produtos { get; set; }

        public async Task OnGetAsync()
        {
            // Leitura sem tracking (melhor performance)
            Produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

_

@page
@model Aula01.Web.Pages.Produtos.IndexModel

<h2>Produtos</h2>

<a asp-page="Create">Novo Produto</a>

<table>
@foreach (var p in Model.Produtos)
{
    <tr>
        <td>@p.Nome</td>
        <td>@p.Preco.ToString("C")</td>
        <td>@p.Categoria?.Nome</td>
        <td>
            <a asp-page="Edit" asp-route-id="@p.Id">Editar</a>
        </td>
    </tr>
}
</table>

_

using Aula01.Web.Data;
using Aula01.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aula01.Web.Pages.Produtos
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Produto Produto { get; set; }

        public List<SelectListItem> Categorias { get; set; }

        public void OnGet()
        {
            Categorias = _context.Categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet(); // Recarrega categorias
                return Page();
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}

_

@page
@model Aula01.Web.Pages.Produtos.CreateModel

<h2>Novo Produto</h2>

<form method="post">
    <partial name="_ProdutoForm" model="Model.Produto" />

    <select asp-for="Produto.CategoriaId"
            asp-items="Model.Categorias">
    </select>

    <button type="submit">Salvar</button>
</form>

_

@model Aula01.Web.Models.Produto

<label>Nome</label>
<input asp-for="Nome" />
<span asp-validation-for="Nome"></span>

<label>Preço</label>
<input asp-for="Preco" />
<span asp-validation-for="Preco"></span>

<label>
    <input asp-for="Ativo" /> Ativo
</label>

_

using Aula01.Web.Data;
using Aula01.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aula01.Web.Pages.Produtos
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Produto Produto { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Produto = await _context.Produtos.FindAsync(id);

            if (Produto == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Update(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}

_

<!DOCTYPE html>
<html>
<head>
    <title>Admin .NET</title>
</head>
<body>
    <header>
        <h1>Sistema Administrativo</h1>
        <hr />
    </header>

    <main>
        @RenderBody()
    </main>
</body>
</html>

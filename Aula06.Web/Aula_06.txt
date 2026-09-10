builder.Services.AddSession();

app.UseSession();

_

TempData["Mensagem"] = "Produto cadastrado com sucesso!";

_

@if (TempData["Mensagem"] != null)
{
    <div class="alert alert-success">
        @TempData["Mensagem"]
    </div>
}

_

public string? Imagem { get; set; }

_


@page
@model Aula01.Web.Pages.Produtos.CreateModel

<form method="post" enctype="multipart/form-data">

    <partial name="_ProdutoForm" model="Model.Produto" />

    <label>Imagem</label>
    <input type="file" name="ImagemUpload" />

    <button type="submit">Salvar</button>
</form>

_

using Microsoft.AspNetCore.Http;

[BindProperty]
public IFormFile? ImagemUpload { get; set; }

public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();

    if (ImagemUpload != null)
    {
        var nomeArquivo = Guid.NewGuid() + Path.GetExtension(ImagemUpload.FileName);
        var caminho = Path.Combine("wwwroot/imagens", nomeArquivo);

        using var stream = new FileStream(caminho, FileMode.Create);
        await ImagemUpload.CopyToAsync(stream);

        Produto.Imagem = nomeArquivo;
    }

    _context.Produtos.Add(Produto);
    await _context.SaveChangesAsync();

    TempData["Mensagem"] = "Produto cadastrado com sucesso!";
    return RedirectToPage("Index");
}

_

<td>
@if (!string.IsNullOrEmpty(p.Imagem))
{
    <img src="~/imagens/@p.Imagem" width="80" />
}
</td>

_

using Microsoft.AspNetCore.Mvc;

namespace Aula01.Web.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

_

<nav>
    <a asp-page="/Produtos/Index">Produtos</a> |
    <a asp-page="/Produtos/Create">Novo Produto</a>
</nav>
<hr />

_

@await Component.InvokeAsync("Menu")



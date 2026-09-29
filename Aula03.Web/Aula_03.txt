using System.ComponentModel.DataAnnotations;

namespace Aula01.Web.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [Range(0.01, 99999)]
        public decimal Preco { get; set; }

        public bool Ativo { get; set; }
    }
}

_

using Aula01.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aula01.Web.Controllers
{
    public class ProdutoController : Controller
    {
        // Simulação de base de dados em memória
        private static List<Produto> _produtos = new();
        private static int _id = 1;

        // LISTAR
        public IActionResult Index()
        {
            return View(_produtos);
        }

        // FORMULÁRIO CRIAÇÃO
        public IActionResult Create()
        {
            return View();
        }

        // SALVAR
        [HttpPost]
        public IActionResult Create(Produto produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            produto.Id = _id++;
            _produtos.Add(produto);

            return RedirectToAction(nameof(Index));
        }

        // FORMULÁRIO EDIÇÃO
        public IActionResult Edit(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            return View(produto);
        }

        // ATUALIZAR
        [HttpPost]
        public IActionResult Edit(Produto produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            var existente = _produtos.First(p => p.Id == produto.Id);
            existente.Nome = produto.Nome;
            existente.Preco = produto.Preco;
            existente.Ativo = produto.Ativo;

            return RedirectToAction(nameof(Index));
        }

        // EXCLUIR
        public IActionResult Delete(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            if (produto != null)
                _produtos.Remove(produto);

            return RedirectToAction(nameof(Index));
        }
    }
}

_

@model List<Aula01.Web.Models.Produto>

<h2>Produtos</h2>

<a asp-action="Create">Novo Produto</a>

<table>
    <tr>
        <th>Nome</th>
        <th>Preço</th>
        <th>Ações</th>
    </tr>

@foreach (var p in Model)
{
    <tr>
        <td>@p.Nome</td>
        <td>@p.Preco.ToString("C")</td>
        <td>
            <a asp-action="Edit" asp-route-id="@p.Id">Editar</a> |
            <a asp-action="Delete" asp-route-id="@p.Id">Excluir</a>
        </td>
    </tr>
}
</table>

_

@model Aula01.Web.Models.Produto

<h2>Novo Produto</h2>

<form asp-action="Create" method="post">
    <label>Nome</label>
    <input asp-for="Nome" />
    <span asp-validation-for="Nome"></span>

    <label>Preço</label>
    <input asp-for="Preco" />
    <span asp-validation-for="Preco"></span>

    <label>
        <input asp-for="Ativo" /> Ativo
    </label>

    <button type="submit">Salvar</button>
</form>

_

@model Aula01.Web.Models.Produto

<h2>Editar Produto</h2>

<form asp-action="Edit" method="post">
    <input type="hidden" asp-for="Id" />

    <label>Nome</label>
    <input asp-for="Nome" />
    <span asp-validation-for="Nome"></span>

    <label>Preço</label>
    <input asp-for="Preco" />
    <span asp-validation-for="Preco"></span>

    <label>
        <input asp-for="Ativo" /> Ativo
    </label>

    <button type="submit">Salvar</button>
</form>

_

<script src="~/lib/jquery/jquery.min.js"></script>
<script src="~/lib/jquery-validation/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>

_

using Microsoft.AspNetCore.Mvc.Filters;

namespace Aula01.Web.Filters
{
    public class LogActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("Executando Action...");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("Action finalizada.");
        }
    }
}

_

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogActionFilter>();
});


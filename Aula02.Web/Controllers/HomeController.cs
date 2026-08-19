namespace Aula02.Web.Controllers;

using Aula02.Web.Interfaces; // Use Aula02.Web.Interfaces diretamente (remova .Services.Interfaces)
using Aula02.Web.Models;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IProdutoService _produtoService;

    public HomeController(IUsuarioService usuarioService, IProdutoService produtoService)
    {
        _usuarioService = usuarioService;
        _produtoService = produtoService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.ObterUsuariosAtivosAsync();
        var produtos = await _produtoService.ObterProdutosAtivosAsync();

        var viewModel = new HomeViewModel
        {
            Usuarios = usuarios,
            Produtos = produtos
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
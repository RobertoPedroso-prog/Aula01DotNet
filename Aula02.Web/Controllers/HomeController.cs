namespace Aula02.Web.Controllers;

using Aula02.Web.Interfaces; // Use Aula02.Web.Interfaces diretamente (remova .Services.Interfaces)
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public HomeController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.ObterUsuariosAtivosAsync();
        return View(usuarios);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
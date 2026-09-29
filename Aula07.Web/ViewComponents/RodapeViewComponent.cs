using Microsoft.AspNetCore.Mvc;

namespace Aula07.Web.ViewComponents;

public class RodapeViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var ultimaPagina = HttpContext.Session.GetString("UltimaPagina");
        return View(model: ultimaPagina);
    }
}

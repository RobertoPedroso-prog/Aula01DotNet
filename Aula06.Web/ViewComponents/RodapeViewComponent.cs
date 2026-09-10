using Microsoft.AspNetCore.Mvc;

namespace Aula06.Web.ViewComponents;

// ViewComponent de rodapé (desafio avaliativo da Aula 6), reutilizável em todo o layout.
public class RodapeViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var ultimaPagina = HttpContext.Session.GetString("UltimaPagina");
        return View(model: ultimaPagina);
    }
}

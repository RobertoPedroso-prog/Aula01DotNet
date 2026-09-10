using Microsoft.AspNetCore.Mvc;

namespace Aula06.Web.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}

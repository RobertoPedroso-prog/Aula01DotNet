using Microsoft.AspNetCore.Mvc;

namespace Aula07.Web.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}

using Microsoft.AspNetCore.Mvc.Filters;

namespace Aula03.Web.Filters;

public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"[LOG] Executando {context.ActionDescriptor.DisplayName}...");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"[LOG] {context.ActionDescriptor.DisplayName} finalizada.");
    }
}

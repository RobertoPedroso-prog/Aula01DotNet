using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aula05.Web.Filters;

// Loga cada acesso às páginas da pasta em que este filtro é registrado (via convention no Program.cs)
public class PageAccessLogFilter : IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("PageAccess");

        logger.LogInformation(
            "Acesso à página {Page} (handler {Handler}) em {Timestamp:u}",
            context.ActionDescriptor.DisplayName,
            context.HandlerMethod?.Name ?? "(sem handler)",
            DateTime.UtcNow);
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }
}

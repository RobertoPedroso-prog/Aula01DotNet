using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aula06.Web.Filters;

// Loga cada acesso às páginas da pasta em que este filtro é registrado (via convention no Program.cs)
// e guarda a última página visitada em Session (desafio avaliativo da Aula 6).
public class PageAccessLogFilter : IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var nomePagina = context.ActionDescriptor.DisplayName ?? "(desconhecida)";

        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("PageAccess");

        logger.LogInformation(
            "Acesso à página {Page} (handler {Handler}) em {Timestamp:u}",
            nomePagina,
            context.HandlerMethod?.Name ?? "(sem handler)",
            DateTime.UtcNow);

        context.HttpContext.Session.SetString("UltimaPagina", nomePagina);
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }
}

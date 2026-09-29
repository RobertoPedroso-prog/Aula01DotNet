using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aula03.Web.Filters;

public class ExecutionTimeFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        await next();
        stopwatch.Stop();

        Console.WriteLine($"[TIMER] {context.ActionDescriptor.DisplayName} executou em {stopwatch.ElapsedMilliseconds}ms");
    }
}

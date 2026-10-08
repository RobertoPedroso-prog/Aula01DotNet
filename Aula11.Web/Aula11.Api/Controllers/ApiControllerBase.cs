using Aula11.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aula11.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Devolve 400 (ProblemDetails) quando page/pageSize são inválidos; null quando está tudo certo.</summary>
    protected ActionResult? PaginacaoInvalida(int page, int pageSize)
    {
        var erro = ProdutoService.ValidarPaginacao(page, pageSize);
        if (erro is null)
            return null;

        ModelState.AddModelError(page < 1 ? nameof(page) : nameof(pageSize), erro);
        return ValidationProblem(ModelState);
    }

    protected ActionResult CategoriaInvalida(string mensagem)
    {
        ModelState.AddModelError("CategoriaId", mensagem);
        return ValidationProblem(ModelState);
    }
}

# Plano — Aula 6: UI Avançada com Razor (TagHelpers, ViewComponents, TempData/Session, Upload)

> **Status: implementado.** Projeto `Aula06.Web` criado do zero (scaffold `dotnet new mvc`), adicionado à solution, reaproveitando o CRUD de Produtos do `Aula05.Web` (schema próprio `aula06`, sem alterar Aula01–05) e acrescentando os recursos da aula: `Session`, `TempData` para mensagens globais, upload de imagem com validação e dois `ViewComponents` (Menu e Rodapé). Desafio avaliativo completo (validação de tipo/tamanho de imagem, ViewComponent de rodapé, última página em Session). Fluxo testado via HTTP real (create com upload válido/inválido/grande, edit preservando imagem, delete, TempData, thumbnails, ViewComponents).

## Fontes analisadas
- `Aula_06.pdf` — slide da aula (teoria de TagHelpers/ViewComponents/TempData-Session/Upload + roteiro da prática + desafio avaliativo)
- `Aula_06.txt` — trechos de código do material original (Session no `Program.cs`, TempData, model `Imagem`, página Create com upload, exibição de imagem no Index, `MenuViewComponent`)

## Contexto e gap identificado
O slide cita "Projeto base: Aula01.Web", mas o `Aula05.Web` já tem o CRUD de Produtos completo e correto (Index/Create/Edit/Delete/Details em Razor Pages, EF Core + PostgreSQL, filtro de log de acesso — ver `Aula05.Web/PLANO.md`), então ele foi a base real portada, não o `Aula01.Web` cru. Os trechos do `.txt` também são incompletos/simplificados perto de código de produção (ex.: caminho `"wwwroot/imagens"` relativo em vez de `IWebHostEnvironment.WebRootPath`, sem validação de tipo/tamanho apesar das "boas práticas" citadas na teoria) — corrigidos na implementação.

## Passo a passo

### 1. Scaffold e infraestrutura
- `dotnet new mvc -n Aula06.Web` + `dotnet sln add`.
- `Aula06.Web.csproj`: pacotes `Npgsql.EntityFrameworkCore.PostgreSQL` e `Microsoft.EntityFrameworkCore.Tools` (mesmas versões do Aula05).
- `Models/Categoria.cs`, `Models/Produto.cs` (+ `public string? Imagem { get; set; }`), `Data/AppDbContext.cs` (schema `aula06`), `Data/DbInitializer.cs` — portados de Aula05.
- `appsettings.json`: mesma connection string do Postgres local (`localhost:54322`), isolado pelo schema.
- Migration `InitialCreate` gerada e aplicada automaticamente no startup via `context.Database.Migrate()` + seed.

### 2. Session
- `builder.Services.AddSession()` + `app.UseSession()` no `Program.cs`, posicionado depois de `UseRouting()` e antes de `UseAuthorization()`/endpoints (senão Session não fica disponível nas Pages).

### 3. TempData para mensagens globais
- `Create`/`Edit`/`Delete` (`Pages/Produtos/*.cshtml.cs`) setam `TempData["Mensagem"]` antes do `RedirectToPage`.
- `Views/Shared/_Layout.cshtml` renderiza o alerta no topo do `<main>`, antes de `@RenderBody()`.

### 4. Upload de imagem
- `Helpers/ImagemUploadHelper.cs`: valida extensão (`.png`/`.jpg`/`.jpeg`) e tamanho (≤ 2 MB), e salva com nome único (`Guid`) em `wwwroot/imagens` — nunca confia no nome original (boas práticas do slide).
- `CreateModel`/`EditModel` recebem `[BindProperty] IFormFile? ImagemUpload`; erro de validação vira `ModelState.AddModelError` (mensagem aparece via `asp-validation-for`).
- `Edit.cshtml` tem um `<input type="hidden" asp-for="Produto.Imagem" />` para a imagem atual não se perder quando nenhum arquivo novo é enviado.
- `Pages/Produtos/Index.cshtml` e `_ProdutoResumo.cshtml` exibem a miniatura (`<img src="~/imagens/@Imagem">`) quando presente.
- `wwwroot/imagens/.gitkeep` criado; `.gitignore` da raiz da solution ganhou uma entrada para não versionar uploads de teste (`Aula06.Web/wwwroot/imagens/*`).

### 5. ViewComponents
- `MenuViewComponent` (`Views/Shared/Components/Menu/Default.cshtml`) — navbar com Home/Produtos/Novo Produto/Privacy, substitui o `<nav>` estático do `_Layout`.
- `RodapeViewComponent` (`Views/Shared/Components/Rodape/Default.cshtml`, desafio item 3) — lê `HttpContext.Session.GetString("UltimaPagina")` e exibe no rodapé, unindo com o item 4 do desafio.

### 6. Última página em Session (desafio item 4)
- `Filters/PageAccessLogFilter.cs` (portado de Aula05, registrado via `AddFolderApplicationModelConvention("/Produtos", ...)`) agora também grava `context.HttpContext.Session.SetString("UltimaPagina", nomePagina)` em `OnPageHandlerExecuting`, além do log original.

## Testes realizados (via `curl`, simulando os formulários reais)
- `GET /Produtos` — lista carrega com seed (Notebook, Livro C#) e miniatura quando há imagem.
- `POST /Produtos/Create` com PNG de 1x1 válido → 302 para Index, alerta "Produto cadastrado com sucesso!", miniatura aparece, arquivo salvo em `wwwroot/imagens` com nome `Guid`.
- `POST /Produtos/Create` com `.txt` → 200 (não redireciona), erro "Apenas imagens PNG ou JPG são permitidas." no campo.
- `POST /Produtos/Create` com PNG de 3 MB → 200, erro "A imagem deve ter no máximo 2 MB."
- `POST /Produtos/Edit/{id}` sem novo arquivo, reenviando o campo oculto `Produto.Imagem` → imagem original preservada, alerta "Produto atualizado com sucesso!".
- `POST /Produtos/Delete/{id}` → produto removido, alerta "Produto excluído com sucesso!".
- `GET /Produtos/Details/{id}` — dados formatados em pt-BR (`R$ 4.500,00`).
- Menu e Rodapé renderizados via `Component.InvokeAsync`; rodapé mostra "Última página visitada: /Produtos/Index" após navegação, confirmando a Session.

## Resultado esperado (do slide)
- ✅ Tela administrativa funcional (CRUD completo reaproveitado de Aula05).
- ✅ Upload e exibição de imagens, com validação de tipo/tamanho.
- ✅ Componentes reutilizáveis (Menu e Rodapé via ViewComponent).
- ✅ Mensagens globais via TempData.
- ✅ UI organizada e escalável, com Session guardando a última página visitada.

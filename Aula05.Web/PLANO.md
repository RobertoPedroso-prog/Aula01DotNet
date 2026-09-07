# Plano — Aula 5: Razor Pages na Prática

> **Status: implementado.** Projeto `Aula05.Web` criado do zero (scaffold `dotnet new mvc`), adicionado à solution, com EF Core + PostgreSQL reaproveitando os models `Produto`/`Categoria` do `Aula04.Web` (schema próprio `aula05`, sem alterar Aula01–04). CRUD completo em Razor Pages testado via HTTP (Create/Edit/Delete/validação) e desafio avaliativo concluído (Delete + partial de confirmação + filtro de log de acesso).
>
> **Bug real encontrado e corrigido durante os testes:** as views `Create.cshtml`/`Edit.cshtml` do material original renderizam a partial `_ProdutoForm` com `model="Model.Produto"`, o que gera inputs sem prefixo (`name="Nome"`), enquanto o `<select>` de Categoria (fora da partial) usa `asp-for="Produto.CategoriaId"` (prefixado). Isso quebra o model binding real do formulário — o POST do navegador nunca preenche `Produto.Nome`/`Produto.Preco`. Corrigido trocando para `<partial name="_ProdutoForm" for="Produto" />`, que propaga o prefixo corretamente. Validado com requisições HTTP reais simulando o form gerado.

## Fontes analisadas
- `Aula_05.pdf` — slide da aula (teoria + roteiro da prática + desafio avaliativo)
- `Aula_05.txt` — código-fonte completo do CRUD de Produtos em Razor Pages (Index, Create, Edit, partial de formulário, layout)

## Contexto e gap identificado
O material assume o projeto **Aula01.Web** como base, mas hoje esse projeto é **MVC puro**:
- `Program.cs` só tem `AddControllersWithViews()` / `MapControllerRoute` — sem `AddRazorPages()`/`MapRazorPages()`.
- Não existe pasta `Data/` nem `AppDbContext`.
- Não existem `Models/Produto.cs` nem `Models/Categoria.cs` (só `ErrorViewModel.cs`).
- Não há pacotes do EF Core no `.csproj`.

Ou seja, antes de colar o código do `.txt`, é preciso preparar a infraestrutura (EF Core + Models + DbContext + habilitar Razor Pages), senão nada compila.

## Objetivo
Implementar o CRUD completo de Produto com Razor Pages em `Aula01.Web`, reproduzindo o conteúdo do `.txt`, e concluir o desafio avaliativo do slide (Delete + filtro de log de acesso).

## Passo a passo

### 1. Infraestrutura (pré-requisito, não coberto explicitamente no .txt)
- Adicionar pacotes NuGet ao `Aula01.Web.csproj`: `Microsoft.EntityFrameworkCore.Sqlite` (ou SqlServer), `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`.
- Criar `Models/Categoria.cs` (Id, Nome, coleção de Produtos).
- Criar `Models/Produto.cs` (Id, Nome, Preco, Ativo, CategoriaId, navegação Categoria) com DataAnnotations (`[Required]`, `[Range]`, etc.) para viabilizar a validação server-side citada no slide.
- Criar `Data/AppDbContext.cs` com `DbSet<Produto>` e `DbSet<Categoria>`.
- Registrar o `AppDbContext` no `Program.cs` (`AddDbContext`) e adicionar `AddRazorPages()` / `MapRazorPages()` ao pipeline, mantendo o MVC existente funcionando lado a lado.
- Criar migration inicial (`dotnet ef migrations add InitialProdutos`) e aplicar (`dotnet ef database update`).
- (Opcional) Seed de algumas categorias para testar o `<select>` de Create.

### 2. Estrutura de pastas Razor Pages
Criar em `Aula01.Web`:
```
Pages/
  Produtos/
    Index.cshtml + Index.cshtml.cs
    Create.cshtml + Create.cshtml.cs
    Edit.cshtml + Edit.cshtml.cs
  Shared/
    _Layout.cshtml
    _ProdutoForm.cshtml
```

### 3. Página de Listagem (Index)
- `IndexModel.OnGetAsync()`: consulta `Produtos.Include(p => p.Categoria).AsNoTracking().ToListAsync()`.
- View lista em tabela (Nome, Preço formatado `"C"`, Categoria, link Editar) + link "Novo Produto".

### 4. Página de Criação (Create)
- `CreateModel` com `[BindProperty] Produto Produto` e `List<SelectListItem> Categorias`.
- `OnGet()` carrega categorias para o `<select>`.
- `OnPostAsync()` valida `ModelState`, recarrega categorias em caso de erro, senão persiste e redireciona para `Index`.
- View usa a partial `_ProdutoForm` + `<select asp-for="Produto.CategoriaId" asp-items="Model.Categorias">`.

### 5. Partial View reutilizável (`_ProdutoForm.cshtml`)
- Campos Nome, Preço, Ativo com `asp-for` + `asp-validation-for`, compartilhada entre Create e Edit.

### 6. Página de Edição (Edit)
- `OnGetAsync(int id)` busca por `FindAsync`, retorna `NotFound()` se nulo.
- `OnPostAsync()` valida e faz `_context.Update(Produto)` + `SaveChangesAsync()`, redireciona para `Index`.

### 7. Layout server-side (`_Layout.cshtml`)
- Estrutura HTML básica com header "Sistema Administrativo" e `@RenderBody()`.
- Adicionar `_ViewStart.cshtml` em `Pages/` apontando para esse layout (não estava no `.txt`, mas é necessário para o layout ser aplicado).

### 8. Validação server-side
- Confirmar que `Produto` tem DataAnnotations coerentes com os campos do form (Nome obrigatório, Preço com `[Range]`, etc.).
- Testar submissão inválida (campo vazio) para checar mensagens de `asp-validation-for`.
- Incluir `_ValidationScriptsPartial` (já existe em `Views/Shared`, reaproveitar ou copiar para `Pages/Shared`) se quiser validação client-side também.

### 9. Testes manuais (rodar a aplicação)
- `dotnet run` no `Aula01.Web`, navegar em `/Produtos`.
- Fluxo: listar → criar produto com categoria → validar erro com campo vazio → editar produto existente → confirmar persistência via `Index`.

### 10. Desafio avaliativo (do slide)
1. Criar página **Delete** (`Pages/Produtos/Delete.cshtml` + `.cs`) com `OnGetAsync(int id)` carregando o produto e `OnPostAsync()` removendo e redirecionando para `Index`.
2. Usar uma **Partial View** para a confirmação de exclusão (ex.: `_ProdutoResumo.cshtml` reutilizando exibição de dados do produto).
3. Implementar um **filtro** (`IPageFilter`/`IAsyncPageFilter` ou middleware) que loga cada acesso às páginas de Produtos (ex.: `_logger.LogInformation` com nome da página e timestamp), registrado via `Program.cs` ou `[TypeFilter]` no `PageModel`.

## Resultado esperado
- CRUD funcional (Create/Read/Update/Delete) em Razor Pages, convivendo com o MVC existente.
- Código organizado por página (PageModel + handlers), com `BindProperty` e validação automática.
- Layout único reutilizado por todas as páginas de Produtos.
- Log de acesso às páginas via filtro, como base para observabilidade nas próximas aulas (Blazor Server, Aula 6).

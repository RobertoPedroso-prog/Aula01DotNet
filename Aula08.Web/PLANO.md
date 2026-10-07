# Plano — Aula 8: Blazor Server

> **Status: implementado.** Projeto `Aula08.Web` criado do zero (scaffold `dotnet new web` + Razor Components), adicionado à solution `Aula01DotNet.sln`, configurado com Blazor Server + EF Core/PostgreSQL isolado no schema `aula08`, reaproveitando os models `Produto`/`Categoria`. O CRUD funcional com componentes reutilizáveis (`ProdutoForm`, `ProdutoList`, `ProdutoDetalhe`, `ConfirmarExclusao`), comunicação entre componentes via `EventCallback` e `CascadingParameter`, state management no serviço e persistência real em banco de dados foram concluídos e validados.

---

## 1. Fontes Analisadas

- **`Aula_08.pdf`**:
  - **Teoria**: Arquitetura do Blazor Server (render server-side, atualização DOM via SignalR), ciclo de vida dos componentes (`OnInitialized`, `OnParametersSet`, `OnAfterRender`), state management local vs. injeção de serviços, comunicação entre componentes (`EventCallback`, `CascadingParameter`), boas práticas.
  - **Prática**: Criação de `ProdutoService` com lista em memória, modelo de componentes reutilizáveis (`ProdutoForm`, `ProdutoList`), orquestração de eventos para adicionar/remover produtos, `StateHasChanged()`.
  - **Atividade Avaliativa (Desafio Prático)**:
    1. Criar componente `ProdutoDetalhe`;
    2. Usar `CascadingParameter` para passar dados;
    3. Implementar confirmação de exclusão;
    4. Persistir estado em serviço.

- **`Aula_08.txt`**:
  - Trechos de código brutos: `Program.cs` com `AddScoped<ProdutoService>()`, classe `ProdutoService` com `List<Produto>`, páginas `.razor`, componentes.

---

## 2. Contexto e Gaps Identificados

1. **Ausência do projeto `Aula08.Web`**:
   - A pasta `Aula08.Web` continha apenas o `.pdf` e `.txt`.
   - A solution `Aula01DotNet.sln` possuía até `Aula07.Web`. Foi necessário criar `Aula08.Web` do zero.

2. **Mudança de paradigma (Razor Pages → Blazor Server)**:
   - Aula05–Aula07 usavam Razor Pages (servidor-side rendering tradicional).
   - Aula08 introduz **Blazor Server** (WebAssembly/InteractiveServer com SignalR).
   - Estrutura de componentes (`.razor`) é completamente diferente de páginas `.cshtml`.

3. **Dados em memória vs. persistência real**:
   - Slide propõe `List<Produto>` em memória dentro do `ProdutoService`.
   - Deciso de design (confirmado com usuário): usar **EF Core + PostgreSQL com schema `aula08`** para persistência real (dados não se perdem ao restart).
   - `ProdutoService` continua expondo a mesma interface do slide (`ObterTodos`, `Adicionar`, `Remover`), mas agora assíncronos e backed por `AppDbContext`.

4. **Ausência de autenticação**:
   - Aula07 implementou Identity (login/roles).
   - Aula08 não menciona autenticação no slide — **não foi portada**; foco exclusivo na arquitetura Blazor Server.

5. **Gaps nas Views/Componentes do `.txt`**:
   - Slide não inclui `ProdutoDetalhe` (Desafio 1).
   - Slide não detalha como usar `CascadingParameter` (Desafio 2).
   - Slide não inclui confirmação de exclusão inline (Desafio 3) — apenas sugere lógica de `Remover`.
   - Slide não exemplifica persistência durável (Desafio 4).

---

## 3. Arquitetura da Solução

```
Aula08.Web/
├── App.razor                        # Root component (HTML host)
├── Routes.razor                     # Router configuration (page routing)
├── _Imports.razor                   # Global using directives
├── Program.cs                       # DI container, middleware
├── appsettings.json                 # DB connection string
├── PLANO.md                         # This file
│
├── Models/
│   ├── Produto.cs                   # Entity: Id, Nome, Preco, Ativo, CategoriaId, Categoria
│   └── Categoria.cs                 # Entity: Id, Nome, Produtos navigation
│
├── Data/
│   ├── AppDbContext.cs              # DbContext for Produto/Categoria, schema 'aula08'
│   └── DbInitializer.cs             # Seed: 2 categorias + 2 produtos
│
├── Services/
│   └── ProdutoService.cs            # Business logic: ObterTodosAsync, AdicionarAsync, RemoverAsync, etc.
│
├── Components/
│   ├── ProdutoForm.razor            # Form to add new products (EditForm with validation)
│   ├── ProdutoList.razor            # Table to list products with Detalhes/Excluir buttons
│   ├── ProdutoDetalhe.razor         # Display selected product details (Desafio 1 + Desafio 2)
│   └── ConfirmarExclusao.razor      # Confirmation dialog for deletion (Desafio 3)
│
├── Layouts/
│   └── MainLayout.razor             # Master layout with navbar/footer
│
├── Pages/
│   ├── Home.razor                   # Landing page (route: /)
│   └── Produtos.razor               # Main CRUD page (route: /produtos)
│
├── wwwroot/
│   ├── app.css                      # Styling
│   └── index.html                   # Entry point
│
└── Migrations/
    ├── [timestamp]_InitialCreate.cs  # EF migration for schema aula08
    └── AppDbContextModelSnapshot.cs
```

---

## 4. Passo a Passo de Execução

### Passo 1: Scaffold do Projeto
1. `dotnet new web -n Aula08.Web -o Aula08.Web --force` (base web simples).
2. `dotnet sln add Aula08.Web/Aula08.Web.csproj` (registrar na solution).
3. Adicionar pacotes ao `.csproj`:
   - `Microsoft.EntityFrameworkCore.Tools`
   - `Npgsql.EntityFrameworkCore.PostgreSQL`

### Passo 2: Configurar Blazor Server
1. Criar `App.razor` (root HTML/layout host).
2. Criar `Routes.razor` (router configuration).
3. Criar `_Imports.razor` (global using directives).
4. Adicionar `MainLayout.razor` em `Layouts/`.
5. Adicionar `Home.razor` e `Produtos.razor` em `Pages/`.

### Passo 3: Models
- Portar `Produto.cs` e `Categoria.cs` de `Aula07.Web`, sem o campo `Imagem`.

### Passo 4: Persistência (Data Layer)
- `AppDbContext.cs`: DbContext com schema `aula08` padrão.
- `DbInitializer.cs`: Seed de 2 categorias + 2 produtos.
- Configurar connection string em `appsettings.json` (PostgreSQL local).

### Passo 5: Serviço (`ProdutoService`)
Reescrever conforme o slide, mas com `AppDbContext` injetado e métodos assíncronos:
- `async Task<List<Produto>> ObterTodosAsync()`
- `async Task<Produto?> ObterPorIdAsync(int id)`
- `async Task<List<Categoria>> ObterCategoriasAsync()`
- `async Task AdicionarAsync(Produto produto)`
- `async Task RemoverAsync(int id)`

### Passo 6: Componentes Reutilizáveis
1. **`ProdutoForm.razor`**: `EditForm` com validação; serve para criar e editar (parâmetro `ProdutoEmEdicao`, título/botões mudam em modo edição); dispara `EventCallback<Produto> OnSalvar` e `OnCancelarEdicao`. A página decide entre `AdicionarAsync` e `AtualizarAsync` pelo `Id`.
2. **`ProdutoList.razor`**: Tabela de produtos; dispara `EventCallback<int> OnSelecionar`, `OnEditar` e `OnExcluirSolicitado`.
3. **`ProdutoDetalhe.razor`** *(Desafio 1)*: Exibe detalhes do produto selecionado via **`[CascadingParameter] Produto? ProdutoSelecionado`** *(Desafio 2)*.
4. **`ConfirmarExclusao.razor`** *(Desafio 3)*: Componente de confirmação inline (não JS alert) com `EventCallback OnConfirmar` / `OnCancelar`.

### Passo 7: Orquestração em `Pages/Produtos.razor`
- `@inject ProdutoService`
- Estado local: `List<Produto> ListaProdutos`, `Produto? ProdutoSelecionado`, `int? IdParaExcluir`.
- `OnInitializedAsync()`: Carrega produtos do serviço.
- `<CascadingValue Value="ProdutoSelecionado">` → `<ProdutoDetalhe />` (Desafio 2).
- Métodos para adicionar, selecionar, solicitar exclusão, confirmar/cancelar.

### Passo 8: Configuração do `Program.cs`
- `AddRazorComponents().AddInteractiveServerComponents()`
- `AddDbContext<AppDbContext>(...UseNpgsql(...))`
- `AddScoped<ProdutoService>()`
- Middleware: `MapRazorComponents<App>().AddInteractiveServerRenderMode()`
- Auto-migrate e seed no startup.

### Passo 9: Migrations
`dotnet ef migrations add InitialCreate --project Aula08.Web`

---

## 5. Roteiro de Testes e Validação

| Cenário | Ação | Resultado Esperado |
| :--- | :--- | :--- |
| **1. Inicialização** | Executar `dotnet run` e acessar `/` | Página inicial (Home) carrega com botão "Ir para Produtos" |
| **2. Listagem de Produtos** | Acessar `/produtos` | Tabela exibe 2 produtos seededados (Notebook, Livro C#) |
| **3. Selecionar Produto** | Clicar em "Detalhes" | `ProdutoDetalhe` exibe Nome/Preço/Categoria/Status via `CascadingParameter` |
| **4. Adicionar Novo Produto** | Preencher `ProdutoForm` e clicar "Salvar" | Novo produto é adicionado à tabela sem reload de página (SignalR) |
| **5. Exclusão com Confirmação** | Clicar em "Excluir" | `ConfirmarExclusao` aparece; clicar "Cancelar" mantém produto; clicar "Sim, excluir" remove |
| **6. Persistência** | Reiniciar a aplicação (`dotnet run`) | Produtos adicionados manualmente persistem (não voltam ao estado inicial) |
| **7. Validação de Dados** | Tentar submeter formulário vazio | Mensagens de erro de validação aparecem (Nome obrigatório, Preço > 0, Categoria obrigatória) |
| **8. Atualizações em Tempo Real** | Abrir `/produtos` em dois abas diferentes e adicionar produto em uma | A outra aba **não** atualiza automaticamente (Blazor Server é por sessão, não real-time entre clientes) |

---

## 6. Atendimento da Atividade Avaliativa (Desafio Prático)

- [x] **1. Criar componente `ProdutoDetalhe`**: Implementado em `Components/ProdutoDetalhe.razor`; exibe Nome, Preço, Categoria, Status.
- [x] **2. Usar `CascadingParameter`**: Em `ProdutoDetalhe.razor`, `[CascadingParameter] Produto? ProdutoSelecionado` recebe valor de `<CascadingValue Value="ProdutoSelecionado">` em `Produtos.razor`.
- [x] **3. Implementar confirmação de exclusão**: Em `ConfirmarExclusao.razor`, componente reutilizável com buttons "Sim, excluir" / "Cancelar", sem `window.confirm` (tudo em Razor/C#).
- [x] **4. Persistir estado em serviço**: `ProdutoService` + `AppDbContext` + EF Core + PostgreSQL schema `aula08` — dados persistem em BD real, não se perdem ao restart.

---

## 7. Decisões de Design

1. **EF Core + PostgreSQL (vs. `List<Produto>`)**:
   - **Por quê**: Mantém consistência com Aula05–Aula07; dados reais não se perdem; educacionalmente demonstra camada de persistência real em aplicações web modernas.
   - **Trade-off**: Ligeiramente mais código que uma lista em memória, mas valor educacional bem maior.

2. **Sem ASP.NET Core Identity**:
   - **Por quê**: Slide de Aula08 não menciona autenticação; adicionar seria escopo além do pedido.
   - **Trade-off**: Aplicação sem controle de acesso — aceitável para este contexto educacional.

3. **Sem upload de imagem**:
   - **Por quê**: Campo `Imagem` de Aula07 não é mencionado em Aula08; adicionaria escopo desnecessário.
   - **Trade-off**: Modelo de Produto fica mais simples (sem tratamento de arquivo).

4. **`CascadingParameter` em `ProdutoDetalhe`**:
   - **Por quê**: Demonstra claramente o padrão pedido (Desafio 2) e evita prop-drilling de `Produtos` → `ProdutoDetalhe`.
   - **Trade-off**: `CascadingValue` null quando nenhum produto é selecionado (esperado).

---

## 8. Possíveis Extensões (Não Implementadas)

- Adicionar autenticação Identity (login/roles).
- Upload de imagem de produto.
- Paginação de listagem.
- Busca/filtro por nome ou categoria.
- Gráficos de preço usando Blazor + library de charting.

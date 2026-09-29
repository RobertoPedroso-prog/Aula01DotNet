# Resumo da Implementação — Aula 7: Autenticação e Autorização

> **Projeto:** `Aula07.Web`  
> **Solution:** `Aula01DotNet.sln`  
> **Status:** Implementado e 100% validado via testes automatizados.

---

## 1. Scaffold e Integração à Solução

- Projeto criado com o comando:
  ```bash
  dotnet new mvc -n Aula07.Web -o Aula07.Web --force
  dotnet sln add Aula07.Web/Aula07.Web.csproj
  ```
- **Pacotes NuGet instalados** em `Aula07.Web.csproj`:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (10.0.12)
  - `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.3)
  - `Microsoft.EntityFrameworkCore.Tools` (10.0.11)

---

## 2. Banco de Dados Isolado (PostgreSQL / Schema `aula07`)

- Conexão com o PostgreSQL local via Supabase Docker (`localhost:54322`, database `postgres`).
- `AppDbContext` herdando de `IdentityDbContext`:
  - Mapeia as tabelas do ASP.NET Core Identity (`AspNetUsers`, `AspNetRoles`, `AspNetUserClaims`, etc.) em conjunto com `Produtos` e `Categorias`.
  - Configurado com isolamento estrito via schema PostgreSQL próprio:
    ```csharp
    modelBuilder.HasDefaultSchema("aula07");
    base.OnModelCreating(modelBuilder);
    ```
    Isso assegura que nenhuma tabela ou dado das Aulas 01 a 06 seja alterado ou corrompido.
- Migration inicial `IdentityInitial` gerada e aplicada automaticamente no startup da aplicação através de `context.Database.Migrate()`.
- Sementes iniciais de banco (`DbInitializer.Seed`) populando categorias (*Eletrônicos*, *Livros*) e produtos iniciais.

---

## 3. Autenticação, Cookies e Segurança (`Program.cs`)

- **Configuração do Identity:**
  ```csharp
  builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
  {
      options.Password.RequireDigit = false;
      options.Password.RequireLowercase = false;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequiredLength = 6;
      options.SignIn.RequireConfirmedAccount = false;
  })
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();
  ```
- **Configuração do Cookie de Aplicação:**
  ```csharp
  builder.Services.ConfigureApplicationCookie(options =>
  {
      options.LoginPath = "/Account/Login";
      options.AccessDeniedPath = "/Account/AcessoNegado";
      options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
      options.SlidingExpiration = true;
  });
  ```
- **Convenções de Autorização no Razor Pages:**
  - `options.Conventions.AuthorizeFolder("/Produtos")`: Proteção global de todas as páginas de produtos para usuários autenticados.
  - `options.Conventions.AllowAnonymousToFolder("/Account")`: Acesso público liberado para login e registro.
- **Pipeline HTTP Ordenado:**
  - `UseRouting()` ➔ `UseSession()` ➔ `UseAuthentication()` ➔ `UseAuthorization()`.
  - Cultura pt-BR configurada globalmente para tratamento correto de valores monetários e decimais com vírgula.

---

## 4. Páginas de Gerenciamento de Conta (`Pages/Account/`)

- **`Login.cshtml` / `Login.cshtml.cs`:**
  - Autenticação via `SignInManager.PasswordSignInAsync`.
  - Suporte a `ReturnUrl` após o login bem-sucedido.
  - Exibição de alertas amigáveis em caso de credenciais inválidas.
  - Quadro com as credenciais padrão de teste diretamente na interface.
- **`Register.cshtml` / `Register.cshtml.cs`:**
  - Cadastro de novo usuário via `UserManager.CreateAsync`.
  - Atribuição automática da role padrão `"Usuario"`.
  - Gravação da **Claim `Departamento`** selecionada no formulário (`TI`, `Operações`, `Vendas` ou `Financeiro`).
- **`Logout.cshtml` / `Logout.cshtml.cs`:**
  - Encerramento da sessão e revogação do cookie de autenticação através de `SignInManager.SignOutAsync()`.
- **`AcessoNegado.cshtml` / `AcessoNegado.cshtml.cs`:**
  - Tela personalizada de erro 403 (*Acesso Negado*), orientando o usuário sobre os perfis necessários para cada funcionalidade.

---

## 5. Atendimento aos 4 Desafios Avaliativos da Aula

- [x] **1. Criar role Gerente:**
  - Implementado em `Data/IdentitySeeder.cs`, criando automaticamente as roles `Admin`, `Gerente` e `Usuario` via `RoleManager<IdentityRole>`.
- [x] **2. Restringir edição de produtos:**
  - Página `Pages/Produtos/Edit.cshtml.cs` protegida com `[Authorize(Roles = "Admin,Gerente")]`.
  - Usuários com perfil `Usuario` são bloqueados e redirecionados para `/Account/AcessoNegado`.
- [x] **3. Criar Claim Departamento:**
  - Adicionada aos usuários no `IdentitySeeder` (`new Claim("Departamento", valor)`).
  - Incluída no formulário de novo cadastro em `RegisterViewModel.cs` e persistida via `_userManager.AddClaimAsync`.
- [x] **4. Exibir Claim no layout:**
  - Integrada no cabeçalho do layout em `Views/Shared/Components/Menu/Default.cshtml`.
  - Exibe dinamicamente uma badge informativa `Depto: <nome>` ao lado das badges de perfil (`Admin`, `Gerente` ou `Usuário`).

---

## 6. UI Condicional por Perfil (`Pages/Produtos/Index.cshtml`)

A visualização e as ações na listagem de produtos adaptam-se dinamicamente conforme os papéis do usuário autenticado:

- **Botão `+ Novo Produto`:** Renderizado apenas para `Admin` (`User.IsInRole("Admin")`).
- **Botão `Editar`:** Renderizado apenas para `Admin` e `Gerente` (`User.IsInRole("Admin") || User.IsInRole("Gerente")`).
- **Botão `Excluir`:** Renderizado apenas para `Admin` (`User.IsInRole("Admin")`).
- **Botão `Detalhes`:** Acessível para todos os perfis autenticados.

---

## 7. Contas Pré-configuradas para Testes

| E-mail | Senha | Role | Claim Departamento | Permissões em Produtos |
| :--- | :--- | :--- | :--- | :--- |
| `admin@aula07.com` | `Admin123!` | **Admin** | `TI` | Criar, Editar, Excluir e Visualizar (Acesso total) |
| `gerente@aula07.com` | `Gerente123!` | **Gerente** | `Operações` | Editar e Visualizar (Create e Delete bloqueados) |
| `usuario@aula07.com` | `Usuario123!` | **Usuario** | `Vendas` | Apenas Visualizar (Create, Edit e Delete bloqueados) |

---

## 8. Validação e Testes Automatizados

A suíte automatizada de testes HTTP reais cobriu todos os cenários com **100% de sucesso**:

1. **Acesso Anônimo:** Tentativa de acessar `/Produtos` redirecionou com status `302` para `/Account/Login?ReturnUrl=%2FProdutos`.
2. **Login de Usuário Comum (`usuario@aula07.com`):** Menu exibiu e-mail, badge `Usuário` e `Depto: Vendas`. Na tabela, apenas a coluna de `Detalhes` ficou visível.
3. **Bloqueio de Ações para Usuário Comum:** Acessos diretos a `/Produtos/Create` e `/Produtos/Edit/1` foram bloqueados com `302` redirecionando para `/Account/AcessoNegado`.
4. **Login de Gerente (`gerente@aula07.com`):** Menu exibiu badge `Gerente` e `Depto: Operações`. O botão `Editar` foi liberado na tabela e o formulário `/Produtos/Edit/1` carregou com status `200`. Acesso a `/Produtos/Create` foi bloqueado com sucesso.
5. **Login de Admin (`admin@aula07.com`):** Menu exibiu badge `Admin` e `Depto: TI`. Acesso irrestrito a Create (`200`), Edit (`200`) e Delete (`200`).
6. **Cadastro de Novo Usuário com Claim:** Registro realizado com departamento `Financeiro`. Ao autenticar com o novo usuário, o menu exibiu perfeitamente `Depto: Financeiro`.

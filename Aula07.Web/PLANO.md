# Plano — Aula 7: Autenticação e Autorização com ASP.NET Core Identity

> **Status: implementado.** Projeto `Aula07.Web` criado do zero (scaffold `dotnet new mvc`), adicionado à solution `Aula01DotNet.sln`, configurado com ASP.NET Core Identity + PostgreSQL isolado no schema `aula07`, reaproveitando o CRUD de Produtos e upload de imagens. Todas as etapas e os 4 desafios avaliativos foram concluídos com sucesso e validados via suíte automatizada de testes HTTP reais (cookies, roles `Admin`/`Gerente`/`Usuario`, claim `Departamento` no layout e no registro, bloqueios granulares em Create/Edit e página personalizada de Acesso Negado).

---

## 1. Fontes Analisadas

- **`Aula_07.pdf`**:
  - **Teoria**: ASP.NET Core Identity (`UserManager`, `SignInManager`, `RoleManager`), Cookies vs JWT (foco em Cookies server-side para Razor Pages), Roles vs Claims, boas práticas de segurança web (hash seguro, anti-forgery token, princípio do menor privilégio).
  - **Prática**: Instalação dos pacotes Identity, configuração do `AppDbContext : IdentityDbContext`, injeção do Identity e cookie no `Program.cs`, migrations do Identity (`IdentityInitial`), páginas de registro (`Register`) e login (`Login`), proteção global de pasta (`AuthorizeFolder("/Produtos")`), proteção por Role (`[Authorize(Roles = "Admin")]`), UI condicional por perfil (`User.IsInRole("Admin")`), seeder de roles (`IdentitySeeder`).
  - **Atividade Avaliativa (Desafio Prático)**:
    1. Criar role **Gerente**;
    2. Restringir edição de produtos (permitir apenas `Admin` e `Gerente`);
    3. Criar Claim **Departamento** (ex.: `TI`, `Operações`, `Vendas`);
    4. Exibir a Claim no layout principal da aplicação.

- **`Aula_07.txt`**:
  - Trechos de código brutos com comandos CLI (`dotnet add package`, `dotnet ef migrations`), classes de páginas (`RegisterModel`, `RegisterViewModel`, `LoginModel`, `LoginViewModel`), configuração de cookie e conventions no `Program.cs`, e classe `IdentitySeeder`.

---

## 2. Contexto e Gaps Identificados

1. **Ausência do projeto `Aula07.Web` na solution**:
   - A pasta `Aula07.Web` atualmente contém apenas o `.pdf` e o `.txt`.
   - A solution `Aula01DotNet.sln` possui apenas os projetos de `Aula01.Web` a `Aula06.Web`. É necessário criar o projeto (`dotnet new mvc -n Aula07.Web`) e registrá-lo na solution.
2. **Citação de `Aula01.Web` como base no material**:
   - O material menciona `namespace Aula01.Web`, porém o projeto de ponta com o CRUD completo de produtos, upload de imagem, formatação pt-BR e ViewComponents já consolidados está em `Aula06.Web`. Utilizaremos essa base moderna portando para o namespace `Aula07.Web`.
3. **Isolamento de banco de dados**:
   - Para não interferir nas aulas anteriores (`aula05`, `aula06`), o `Aula07.Web` utilizará o schema próprio PostgreSQL **`aula07`** no banco Supabase local (`localhost:54322`).
   - O `AppDbContext` herdará de `IdentityDbContext` e deverá chamar obrigatoriamente `base.OnModelCreating(modelBuilder)` para que as tabelas de Identity (`AspNetUsers`, `AspNetRoles`, etc.) sejam mapeadas dentro do schema `aula07`.
4. **Gaps nas páginas e fluxos de autenticação do `.txt`**:
   - O `.txt` não inclui a página de **Logout** (essencial para alternar entre usuários de diferentes perfis durante os testes).
   - O `.txt` não inclui a página de **Acesso Negado** (`/Account/AcessoNegado`), configurada em `options.AccessDeniedPath`.
   - O `.txt` não inclui as Views `.cshtml` das páginas de Login e Registro, contendo apenas o `PageModel` C#.
   - As políticas de senha padrão do Identity exigem dígitos, letras maiúsculas, minúsculas e caracteres especiais. Configuraremos opções amigáveis ou senhas pré-definidas no seed para facilitar testes sem sacrificar a segurança.

---

## 3. Arquitetura da Solução

```
Aula07.Web/
├── Controllers/
│   └── HomeController.cs                 # Páginas públicas (Home, Privacy)
├── Data/
│   ├── AppDbContext.cs                   # Herda de IdentityDbContext, schema 'aula07'
│   ├── DbInitializer.cs                  # Seed de Categorias e Produtos iniciais
│   └── IdentitySeeder.cs                 # Seed de Roles (Admin, Gerente, Usuario), Claims e Usuários padrão
├── Helpers/
│   └── ImagemUploadHelper.cs             # Validação e upload de imagens (portado de Aula06)
├── Models/
│   ├── Categoria.cs                      # Entidade Categoria
│   ├── Produto.cs                        # Entidade Produto (com Imagem)
│   ├── ErrorViewModel.cs
│   ├── LoginViewModel.cs                 # ViewModel de login
│   └── RegisterViewModel.cs              # ViewModel de registro (+ campo Departamento)
├── Pages/
│   ├── Account/
│   │   ├── Login.cshtml + .cs            # Formulário e lógica de autenticação com cookies
│   │   ├── Register.cshtml + .cs         # Cadastro de usuário, atribuição de Role e Claim
│   │   ├── Logout.cshtml + .cs           # Desconexão (SignOutAsync)
│   │   └── AcessoNegado.cshtml + .cs     # Tela amigável quando falta permissão (403)
│   ├── Produtos/
│   │   ├── Index.cshtml + .cs            # Listagem (acesso: qualquer usuário autenticado; botões condicionais)
│   │   ├── Create.cshtml + .cs           # [Authorize(Roles = "Admin")]
│   │   ├── Edit.cshtml + .cs             # [Authorize(Roles = "Admin,Gerente")] (Desafio 2)
│   │   ├── Delete.cshtml + .cs           # [Authorize(Roles = "Admin")]
│   │   └── Details.cshtml + .cs          # [Authorize]
│   └── Shared/
│       ├── _ProdutoForm.cshtml
│       └── _ProdutoResumo.cshtml
├── ViewComponents/
│   ├── MenuViewComponent.cs              # Navbar com login/logout, roles, e claim Departamento (Desafio 4)
│   └── RodapeViewComponent.cs            # Rodapé informativo
├── Views/
│   └── Shared/
│       └── _Layout.cshtml                # Layout mestre integrando menu e alertas TempData
├── appsettings.json                      # Connection string do PostgreSQL local
└── Program.cs                            # Configuração de Identity, Cookies, Authorization e Pipeline
```

---

## 4. Passo a Passo de Execução

### Passo 1: Scaffold do Projeto e Adição à Solution
1. Executar no diretório raiz:
   ```bash
   dotnet new mvc -n Aula07.Web -o Aula07.Web --force
   dotnet sln add Aula07.Web/Aula07.Web.csproj
   ```
2. Adicionar os pacotes necessários ao `Aula07.Web.csproj`:
   - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
   - `Npgsql.EntityFrameworkCore.PostgreSQL`
   - `Microsoft.EntityFrameworkCore.Tools`

### Passo 2: Configuração do DbContext e PostgreSQL
1. Criar `Data/AppDbContext.cs`:
   - Herdar de `IdentityDbContext<IdentityUser>` (ou `IdentityDbContext`).
   - Mapear `DbSet<Produto>` e `DbSet<Categoria>`.
   - No `OnModelCreating`:
     - Definir schema padrão: `modelBuilder.HasDefaultSchema("aula07");`
     - Chamar `base.OnModelCreating(modelBuilder);` (fundamental para as tabelas do Identity).
2. Configurar `appsettings.json` com a string de conexão para `localhost:54322` (Supabase local).

### Passo 3: Configuração do Identity e Pipeline no `Program.cs`
1. Configurar serviços do Identity:
   ```csharp
   builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
   {
       options.Password.RequireDigit = false;
       options.Password.RequireNonAlphanumeric = false;
       options.Password.RequireUppercase = false;
       options.Password.RequiredLength = 6;
   })
   .AddEntityFrameworkStores<AppDbContext>()
   .AddDefaultTokenProviders();
   ```
2. Configurar cookies de aplicação:
   ```csharp
   builder.Services.ConfigureApplicationCookie(options =>
   {
       options.LoginPath = "/Account/Login";
       options.AccessDeniedPath = "/Account/AcessoNegado";
   });
   ```
3. Configurar convenções de Razor Pages:
   ```csharp
   builder.Services.AddRazorPages(options =>
   {
       options.Conventions.AuthorizeFolder("/Produtos");
       options.Conventions.AllowAnonymousToFolder("/Account");
   });
   ```
4. Ordenar corretamente o pipeline HTTP:
   - `app.UseRouting();`
   - `app.UseSession();`
   - `app.UseAuthentication();` *(essencial antes de UseAuthorization)*
   - `app.UseAuthorization();`

### Passo 4: Migrations do EF Core + Identity
1. Gerar a migration inicial do Identity e produtos:
   ```bash
   dotnet ef migrations add IdentityInitial --project Aula07.Web
   ```
2. Aplicar automaticamente as migrations e sementes no `Program.cs`:
   - `context.Database.Migrate()`
   - `DbInitializer.Seed(context)` (categorias e produtos)
   - `IdentitySeeder.SeedAsync(scope.ServiceProvider)`

### Passo 5: IdentitySeeder com Roles, Claims e Usuários de Teste
Implementar `Data/IdentitySeeder.cs`:
- Criar Roles: `"Admin"`, `"Gerente"`, `"Usuario"`.
- Criar contas pré-configuradas para validação rápida dos perfis:
  - **Admin**: `admin@aula07.com` (Senha: `Admin123!`, Role: `Admin`, Claim: `Departamento` = `TI`)
  - **Gerente**: `gerente@aula07.com` (Senha: `Gerente123!`, Role: `Gerente`, Claim: `Departamento` = `Operações`)
  - **Usuário**: `usuario@aula07.com` (Senha: `Usuario123!`, Role: `Usuario`, Claim: `Departamento` = `Vendas`)

### Passo 6: Criação das Telas de Conta (`Pages/Account/`)
1. **`Register.cshtml` / `.cs`**:
   - Campos: Email, Senha, Confirmação de Senha e Seleção/Input de **Departamento** (Claim).
   - Ao registrar com sucesso:
     - Associa a Role default `"Usuario"`;
     - Grava a Claim `new Claim("Departamento", Input.Departamento)`;
     - Redireciona para `/Account/Login` com mensagem no `TempData`.
2. **`Login.cshtml` / `.cs`**:
   - Campos: Email, Senha, Lembrar-me.
   - Executa `_signInManager.PasswordSignInAsync`.
   - Redireciona para o `returnUrl` solicitado ou `/Produtos/Index`.
3. **`Logout.cshtml` / `.cs`**:
   - `OnPostAsync` / `OnGetAsync` executa `_signInManager.SignOutAsync()`.
   - Redireciona para `/Account/Login`.
4. **`AcessoNegado.cshtml` / `.cs`**:
   - Interface estilizada com Bootstrap avisando que o perfil não possui permissão para acessar o recurso.

### Passo 7: Portabilidade do CRUD e Regras de Autorização
Portar `Pages/Produtos` de `Aula06.Web` com proteções declarativas:
- `Index.cshtml.cs`: protegido pelo `AuthorizeFolder("/Produtos")`.
- `Create.cshtml.cs`: `[Authorize(Roles = "Admin")]`.
- `Edit.cshtml.cs`: `[Authorize(Roles = "Admin,Gerente")]` *(Atende ao Desafio 2)*.
- `Delete.cshtml.cs`: `[Authorize(Roles = "Admin")]`.
- `Details.cshtml.cs`: acessível a qualquer usuário autenticado.

### Passo 8: Exibição Condicional na UI e Exibição de Claims no Layout
1. **Em `Pages/Produtos/Index.cshtml`**:
   - Botão **"Novo Produto"**: envolto em `@if (User.IsInRole("Admin")) { ... }`.
   - Botão **"Editar"**: envolto em `@if (User.IsInRole("Admin") || User.IsInRole("Gerente")) { ... }`.
   - Botão **"Excluir"**: envolto em `@if (User.IsInRole("Admin")) { ... }`.
2. **No Menu / Layout (`MenuViewComponent` & `_Layout.cshtml`)**:
   - Quando não autenticado: exibir links para **Entrar (Login)** e **Cadastrar (Registro)**.
   - Quando autenticado:
     - Exibir o email do usuário;
     - Exibir badges estilizadas com as Roles (`Admin`, `Gerente`, etc.);
     - **Exibir a Claim `Departamento`** via `User.FindFirst("Departamento")?.Value` *(Atende ao Desafio 4)*;
     - Botão de **Logout** (Sair).

---

## 5. Roteiro de Testes e Validação

| Cenário de Teste | Ação / Requisição | Resultado Esperado |
| :--- | :--- | :--- |
| **1. Não Autenticado** | Tentar acessar `GET /Produtos` | 302 Redireciona para `/Account/Login?ReturnUrl=%2FProdutos` |
| **2. Login Usuário Comum** | POST `/Account/Login` com `usuario@aula07.com` | 302 Redireciona para `/Produtos/Index`. Navbar exibe badge `Usuario` e departamento `Vendas`. |
| **3. Acesso Negado Create** | Usuário comum tenta acessar `GET /Produtos/Create` | 302 Redireciona para `/Account/AcessoNegado` |
| **4. Acesso Negado Edit** | Usuário comum tenta acessar `GET /Produtos/Edit/1` | 302 Redireciona para `/Account/AcessoNegado` |
| **5. Login Gerente** | Login com `gerente@aula07.com` | Navbar exibe badge `Gerente` e departamento `Operações`. |
| **6. Gerente Edita Produto** | Gerente acessa `GET` e `POST /Produtos/Edit/1` | Permissão concedida (200 / 302). Produto é atualizado com sucesso. |
| **7. Gerente Tenta Criar** | Gerente tenta acessar `GET /Produtos/Create` | 302 Redireciona para `/Account/AcessoNegado`. |
| **8. Login Admin** | Login com `admin@aula07.com` | Navbar exibe badge `Admin` e departamento `TI`. Acesso total (Create, Edit, Delete). |
| **9. Registro com Claim** | POST `/Account/Register` com novo email e depto | Usuário criado, role `Usuario` e claim `Departamento` atribuídas. |
| **10. Logout** | POST `/Account/Logout` | Sessão encerrada, cookie revogado, redireciona para login. |

---

## 6. Atendimento da Atividade Avaliativa (Desafio Prático)

- [x] **1. Criar role Gerente**: Implementado no `IdentitySeeder` e atribuível via `RoleManager<IdentityRole>`.
- [x] **2. Restringir edição de produtos**: Anotado com `[Authorize(Roles = "Admin,Gerente")]` no `EditModel` e protegido na UI da tabela `Index.cshtml`.
- [x] **3. Criar Claim Departamento**: Adicionado aos usuários seedados e incluído no formulário de registro (`RegisterViewModel`).
- [x] **4. Exibir Claim no layout**: Renderizado no cabeçalho/navbar através do `MenuViewComponent` e `_Layout.cshtml`.

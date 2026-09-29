# Aula 4 — Entity Framework Core — Implementação

Registro do que foi feito no projeto `Aula04.Web` para a Aula 4 (EF Core), com base no roteiro de `Aula_04.pdf` / `Aula_04.txt`.

## Banco de dados

- **PostgreSQL via Supabase local (Docker)**, container `supabase_db_redesaciar`, porta `54322`.
- Credenciais padrão do Supabase CLI: banco `postgres`, usuário `postgres`, senha `postgres`.
- Como esse Postgres é **compartilhado** com outro projeto (`redesaciar`), as tabelas da aula ficam isoladas no **schema `aula04`** (em vez do `public`), configurado via `modelBuilder.HasDefaultSchema("aula04")`.
- Connection string em `appsettings.json`:
  ```
  Host=localhost;Port=54322;Database=postgres;Username=postgres;Password=postgres
  ```

## Pacotes NuGet adicionados

- `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.3)
- `Microsoft.EntityFrameworkCore.Tools` (10.0.11)
- Ferramenta global `dotnet-ef` instalada (`dotnet tool install --global dotnet-ef`) — necessária para rodar `dotnet ef ...`.

## Parte principal — Produto / Categoria (1:N)

Arquivos criados:
- `Models/Categoria.cs` — `Id`, `Nome`, navegação `List<Produto> Produtos`.
- `Models/Produto.cs` — `Id`, `Nome` (`[Required]`, `StringLength`), `Preco` (`[Range]`), `Ativo`, FK `CategoriaId` + navegação `Categoria? Categoria`.
- `Data/AppDbContext.cs` — `DbSet<Produto>`, `DbSet<Categoria>`; `OnModelCreating` configura schema `aula04` e o relacionamento 1:N (`Categoria.HasMany(Produtos).WithOne(Categoria).HasForeignKey(CategoriaId)`).
- `Data/DbInitializer.cs` — seed automático: categorias "Eletrônicos"/"Livros" e produtos "Notebook"/"Livro C#", só roda se `Categorias` estiver vazio.
- `Controllers/ProdutoController.cs` — CRUD completo com EF Core:
  - `Index`: `Include(p => p.Categoria).AsNoTracking()`.
  - `Create` (GET/POST): popula `ViewBag.Categorias` para o `<select>`.
  - `Edit` (GET/POST) e `Delete` (POST): com `[ValidateAntiForgeryToken]`.
- `Views/Produto/Index.cshtml`, `Create.cshtml`, `Edit.cshtml` — Bootstrap, `<select asp-for="CategoriaId">` no lugar do antigo `<input>` de texto livre.

`Program.cs`:
- `builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(...))`.
- No startup: `context.Database.Migrate()` (aplica migrations pendentes automaticamente) + `DbInitializer.Seed(context)`, dentro de um `using var scope = app.Services.CreateScope()`.

Migration: `InitialCreate` — cria schema `aula04`, tabelas `Categorias` e `Produtos` (com FK `CategoriaId` e índice).

## Desafio — Pedido × Produto (N:N)

Arquivos criados/alterados:
- `Models/Pedido.cs` — `Id`, `DataPedido`, navegação `List<Produto> Produtos`.
- `Models/Produto.cs` — adicionada navegação reversa `List<Pedido> Pedidos`.
- `Data/AppDbContext.cs` — adicionado `DbSet<Pedido>` e configuração N:N:
  ```csharp
  modelBuilder.Entity<Pedido>()
      .HasMany(p => p.Produtos)
      .WithMany(p => p.Pedidos)
      .UsingEntity(j => j.ToTable("PedidoProduto"));

  modelBuilder.Entity<Pedido>()
      .Property(p => p.DataPedido)
      .HasColumnType("timestamp without time zone");
  ```
- `Controllers/PedidoController.cs`:
  - `Index`: lista pedidos com `Include(p => p.Produtos)`, ordenado por data decrescente.
  - `Create` (GET): `ViewBag.Produtos` só com produtos ativos.
  - `Create` (POST): recebe `DateTime dataPedido` + `int[] produtoIds`; valida que ao menos um produto foi selecionado; busca os `Produto`s pelos ids e monta o `Pedido`.
- `Views/Pedido/Index.cshtml` — tabela com data, lista de produtos do pedido e total (soma dos preços).
- `Views/Pedido/Create.cshtml` — campo de data + `<select multiple>` de produtos.
- `Views/Shared/_Layout.cshtml` — links de navegação "Produtos" e "Pedidos" adicionados ao menu.

Migration: `AddPedidoProdutoManyToMany` — cria `Pedidos` e a tabela de junção `PedidoProduto` (chave composta `PedidosId`+`ProdutosId`, FKs em cascata).

### Problema encontrado e correção

Primeira tentativa de salvar um Pedido deu erro:
```
System.ArgumentException: Cannot write DateTime with Kind=Unspecified to PostgreSQL type
'timestamp with time zone', only UTC is supported.
```
Causa: o Npgsql exige `DateTimeKind.Utc` para colunas `timestamp with time zone` (tipo padrão do EF Core para `DateTime`), e o valor vindo do formulário HTML tem `Kind=Unspecified`.

Correção: mapear `DataPedido` como `timestamp without time zone` (Fluent API acima), que aceita qualquer `DateTimeKind`. Foi necessário reverter a migration (`dotnet ef database update InitialCreate`), remover o arquivo (`dotnet ef migrations remove`), corrigir o model e recriar a migration.

## Como rodar / verificar

1. Confirmar que o container do Postgres está de pé: `docker ps` → `supabase_db_redesaciar` (porta `54322`).
2. `dotnet build Aula04.Web` — deve compilar sem erros/avisos.
3. `dotnet ef database update` (dentro de `Aula04.Web/`) — aplica migrations pendentes (ou deixar o `Program.cs` aplicar sozinho no startup via `Database.Migrate()`).
4. `dotnet run` e acessar:
   - `/Produto` — lista, cria, edita e exclui produtos vinculados a uma categoria.
   - `/Pedido` — lista pedidos com os produtos e total; cria pedido selecionando vários produtos.

Testado de ponta a ponta via `curl` (com token anti-forgery e cookies de sessão): Create/Edit/Delete de Produto e Create de Pedido com múltiplos produtos, todos persistindo corretamente no banco.

## Fora do escopo

- Views de `Edit`/`Delete` para `Pedido` não foram pedidas no desafio ("criar migration e exibir pedidos com produtos") e não foram implementadas.

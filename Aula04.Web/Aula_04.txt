dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
_

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

_

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=Aula01Db;Trusted_Connection=True;TrustServerCertificate=True"
}

_

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=aula01db;Username=postgres;Password=123456"
}

_

using Aula01.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Aula01.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração de relacionamento 1:N
            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Produtos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

_

namespace Aula01.Web.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        // Navegação
        public List<Produto> Produtos { get; set; }
    }
}

_

using System.ComponentModel.DataAnnotations;

namespace Aula01.Web.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public decimal Preco { get; set; }
        public bool Ativo { get; set; }

        // FK
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}

_

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Para PostgreSQL:
// options.UseNpgsql(...)

_

dotnet ef migrations add InitialCreate
dotnet ef database update


_

using Aula01.Web.Models;

namespace Aula01.Web.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Categorias.Any())
                return;

            var categorias = new List<Categoria>
            {
                new Categoria { Nome = "Eletrônicos" },
                new Categoria { Nome = "Livros" }
            };

            context.Categorias.AddRange(categorias);
            context.SaveChanges();

            var produtos = new List<Produto>
            {
                new Produto { Nome = "Notebook", Preco = 4500, Ativo = true, CategoriaId = categorias[0].Id },
                new Produto { Nome = "Livro C#", Preco = 120, Ativo = true, CategoriaId = categorias[1].Id }
            };

            context.Produtos.AddRange(produtos);
            context.SaveChanges();
        }
    }
}


_

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}


_


using Aula01.Web.Data;
using Aula01.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aula01.Web.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .AsNoTracking()
                .ToListAsync();

            return View(produtos);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

_

@model Aula01.Web.Models.Produto

<form asp-action="Create" method="post">

    <input asp-for="Nome" />
    <input asp-for="Preco" />

    <select asp-for="CategoriaId"
            asp-items="@(new SelectList(ViewBag.Categorias, "Id", "Nome"))">
    </select>

    <button type="submit">Salvar</button>
</form>


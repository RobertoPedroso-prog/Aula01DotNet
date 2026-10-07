using Aula09.Web.Models;
using Microsoft.EntityFrameworkCore;
using Configuracao = Hipermidia.Data.Entities.Configuracao;

namespace Aula09.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Configuracao> Configuracoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("hipermidia");

        modelBuilder.Entity<Configuracao>().HasKey(c => c.Chave);

        modelBuilder.Entity<Categoria>()
            .HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId);

        base.OnModelCreating(modelBuilder);
    }
}

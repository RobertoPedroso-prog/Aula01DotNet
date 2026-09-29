using Aula04.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Aula04.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("aula04");

        // Configuração de relacionamento 1:N
        modelBuilder.Entity<Categoria>()
            .HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId);

        // Configuração de relacionamento N:N (tabela de junção PedidoProduto)
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Produtos)
            .WithMany(p => p.Pedidos)
            .UsingEntity(j => j.ToTable("PedidoProduto"));

        modelBuilder.Entity<Pedido>()
            .Property(p => p.DataPedido)
            .HasColumnType("timestamp without time zone");

        base.OnModelCreating(modelBuilder);
    }
}

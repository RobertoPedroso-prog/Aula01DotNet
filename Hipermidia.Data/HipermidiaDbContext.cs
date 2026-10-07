using Hipermidia.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hipermidia.Data;

/// <summary>
/// Modelo único do schema "hipermidia", usado por todas as aulas (04 a 10).
/// É o dono das migrations; os DbContext de cada aula apenas mapeiam as mesmas tabelas.
/// </summary>
public class HipermidiaDbContext : IdentityDbContext
{
    public const string Schema = "hipermidia";

    public HipermidiaDbContext(DbContextOptions<HipermidiaDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Configuracao> Configuracoes => Set<Configuracao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Identity (Aula 07): AspNetUsers, AspNetRoles, ...
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(e =>
        {
            e.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Produto>(e =>
        {
            e.Property(p => p.Nome).IsRequired().HasMaxLength(200);
            e.Property(p => p.Preco).HasPrecision(10, 2);
            e.Property(p => p.Imagem).HasMaxLength(500);

            // Defaults no banco: aulas cujo modelo não tem essas colunas continuam inserindo normalmente.
            e.Property(p => p.Ativo).HasDefaultValue(true);
            e.Property(p => p.Estoque).HasDefaultValue(0);

            e.HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId);
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.Property(p => p.DataPedido).HasColumnType("timestamp without time zone");

            e.HasMany(p => p.Produtos)
                .WithMany(p => p.Pedidos)
                .UsingEntity(j => j.ToTable("PedidoProduto"));
        });

        modelBuilder.Entity<Configuracao>(e =>
        {
            e.HasKey(c => c.Chave);
            e.Property(c => c.Chave).HasMaxLength(100);
            e.Property(c => c.Valor).IsRequired().HasMaxLength(500);
        });
    }
}

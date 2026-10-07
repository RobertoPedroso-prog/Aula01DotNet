using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hipermidia.Data;

/// <summary>Cria/atualiza o schema "hipermidia". Cada aula chama <see cref="Aplicar"/> no startup.</summary>
public static class HipermidiaMigrator
{
    public static DbContextOptions<HipermidiaDbContext> CriarOpcoes(string connectionString) =>
        new DbContextOptionsBuilder<HipermidiaDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", HipermidiaDbContext.Schema))
            .Options;

    public static void Aplicar(string? connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        using var context = new HipermidiaDbContext(CriarOpcoes(connectionString));
        context.Database.Migrate();
    }
}

/// <summary>Usada só pelo "dotnet ef": lê a conexão da variável de ambiente HIPERMIDIA_CONNECTION.</summary>
public class HipermidiaDesignTimeFactory : IDesignTimeDbContextFactory<HipermidiaDbContext>
{
    public HipermidiaDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("HIPERMIDIA_CONNECTION")
            ?? "Host=localhost;Port=54322;Database=postgres;Username=postgres";
        return new HipermidiaDbContext(HipermidiaMigrator.CriarOpcoes(cs));
    }
}

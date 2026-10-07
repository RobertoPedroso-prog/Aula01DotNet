using Aula09.Web.Data;
using Hipermidia.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aula09.Web.Services;

/// <summary>Tema (claro/escuro) da aplicação, persistido na tabela hipermidia."Configuracoes".</summary>
public class TemaService
{
    private const string Chave = "tema";

    private readonly IDbContextFactory<AppDbContext> _factory;
    private bool _carregado;

    public TemaService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public string Tema { get; private set; } = "light";

    public event Action? OnTemaChanged;

    public async Task CarregarAsync()
    {
        if (_carregado)
            return;
        _carregado = true;

        using var context = _factory.CreateDbContext();
        var salvo = await context.Configuracoes.FindAsync(Chave);

        if (salvo is { Valor: "light" or "dark" } && salvo.Valor != Tema)
        {
            Tema = salvo.Valor;
            OnTemaChanged?.Invoke();
        }
    }

    public async Task AlterarTemaAsync(string tema)
    {
        if (Tema == tema)
            return;

        Tema = tema;
        OnTemaChanged?.Invoke();

        using var context = _factory.CreateDbContext();
        var configuracao = await context.Configuracoes.FindAsync(Chave);
        if (configuracao is null)
            context.Configuracoes.Add(new Configuracao { Chave = Chave, Valor = tema });
        else
            configuracao.Valor = tema;

        await context.SaveChangesAsync();
    }
}

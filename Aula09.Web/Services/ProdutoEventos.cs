namespace Aula09.Web.Services;

/// <summary>
/// Notificador singleton: avisa todos os circuitos Blazor abertos quando os produtos mudam,
/// para o dashboard atualizar em tempo real.
/// </summary>
public class ProdutoEventos
{
    public event Action? OnProdutosAlterados;

    public void Notificar() => OnProdutosAlterados?.Invoke();
}

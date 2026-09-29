namespace Aula09.Web.Services;

public class TemaService
{
    private string _tema = "light";
    public string Tema => _tema;

    public event Action OnTemaChanged;

    public void AlterarTema(string tema)
    {
        if (_tema != tema)
        {
            _tema = tema;
            OnTemaChanged?.Invoke();
        }
    }
}

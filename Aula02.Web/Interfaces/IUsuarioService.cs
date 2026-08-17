using Aula02.Web.DTOs;

namespace Aula02.Web.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObterUsuariosAtivosAsync();
}

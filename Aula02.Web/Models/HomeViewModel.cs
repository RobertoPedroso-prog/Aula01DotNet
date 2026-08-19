using Aula02.Web.DTOs;

namespace Aula02.Web.Models;

public class HomeViewModel
{
    public required List<UsuarioDto> Usuarios { get; set; }
    public required List<ProdutoDto> Produtos { get; set; }
}

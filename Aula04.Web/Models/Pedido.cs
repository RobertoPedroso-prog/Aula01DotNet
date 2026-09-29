namespace Aula04.Web.Models;

public class Pedido
{
    public int Id { get; set; }

    public DateTime DataPedido { get; set; } = DateTime.Now;

    // N:N
    public List<Produto> Produtos { get; set; } = [];
}

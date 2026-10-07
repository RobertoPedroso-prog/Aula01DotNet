namespace Hipermidia.Data.Entities;

public class Pedido
{
    public int Id { get; set; }

    public DateTime DataPedido { get; set; } = DateTime.Now;

    public List<Produto> Produtos { get; set; } = [];
}

namespace Hipermidia.Data.Entities;

/// <summary>Pares chave/valor de configuração da aplicação (ex.: tema escolhido na Aula 09).</summary>
public class Configuracao
{
    public required string Chave { get; set; }

    public required string Valor { get; set; }
}

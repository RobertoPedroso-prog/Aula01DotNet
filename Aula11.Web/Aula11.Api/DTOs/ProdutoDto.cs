namespace Aula11.Api.DTOs;

/// <summary>Contrato da v1: o que o cliente enxerga de um produto.</summary>
public record ProdutoDto(int Id, string Nome, decimal Preco);

/// <summary>Contrato da v2: a v1 mais Ativo, Estoque e Categoria (campos extras). A v1 continua igual para não quebrar clientes antigos.</summary>
public record ProdutoV2Dto(int Id, string Nome, decimal Preco, bool Ativo, int Estoque, string? Categoria);

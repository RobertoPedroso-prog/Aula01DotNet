using System.Net.Http.Json;
using System.Text.Json;
using Aula11.BlazorWasm.Models;

namespace Aula11.BlazorWasm.Services;

/// <summary>Resultado de uma operação da API: sucesso (Valor) ou uma mensagem de erro pronta para exibir.</summary>
public record Resposta<T>(T? Valor, string? Erro)
{
    public bool Sucesso => Erro is null;
}

/// <summary>Único ponto que fala HTTP com a API de produtos (versões "v1" e "v2" na URL).</summary>
public class ProdutosApiClient
{
    private readonly HttpClient _http;

    public ProdutosApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<Resposta<PagedResult<ProdutoDto>>> ListarAsync(
        string versao, int page, int pageSize, string? nome, bool? ativo)
    {
        var url = $"api/{versao}/produtos?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(nome))
            url += $"&nome={Uri.EscapeDataString(nome.Trim())}";

        if (ativo.HasValue)
            url += $"&ativo={(ativo.Value ? "true" : "false")}";

        try
        {
            using var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new(null, await LerErroAsync(response));

            var resultado = await response.Content.ReadFromJsonAsync<PagedResult<ProdutoDto>>();
            return new(resultado ?? new PagedResult<ProdutoDto>(), null);
        }
        catch (HttpRequestException)
        {
            return new(null, "Não foi possível falar com a API. Verifique se ela está no ar e tente novamente.");
        }
    }

    public async Task<Resposta<ProdutoDto>> CriarAsync(string versao, CriarProdutoRequest request)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync($"api/{versao}/produtos", request);
            if (!response.IsSuccessStatusCode)
                return new(null, await LerErroAsync(response));

            return new(await response.Content.ReadFromJsonAsync<ProdutoDto>(), null);
        }
        catch (HttpRequestException)
        {
            return new(null, "Não foi possível falar com a API. Verifique se ela está no ar e tente novamente.");
        }
    }

    // Traduz o ProblemDetails da API ("errors": { campo: [mensagens] }) em um texto legível.
    private static async Task<string> LerErroAsync(HttpResponseMessage response)
    {
        try
        {
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                var mensagens = errors.EnumerateObject()
                    .SelectMany(e => e.Value.EnumerateArray().Select(m => m.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m));

                var texto = string.Join(" ", mensagens);
                if (texto.Length > 0)
                    return texto;
            }
        }
        catch (JsonException)
        {
        }

        return $"A API respondeu {(int)response.StatusCode} {response.ReasonPhrase}.";
    }
}

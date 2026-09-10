namespace Aula06.Web.Helpers;

// Boas práticas de upload citadas na Aula 6: validar extensão, limitar tamanho
// e nunca confiar no nome original do arquivo (gera um nome novo com Guid).
public static class ImagemUploadHelper
{
    private static readonly string[] ExtensoesPermitidas = [".png", ".jpg", ".jpeg"];
    private const long TamanhoMaximoBytes = 2 * 1024 * 1024; // 2 MB

    public static string? Validar(IFormFile arquivo)
    {
        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

        if (!ExtensoesPermitidas.Contains(extensao))
            return "Apenas imagens PNG ou JPG são permitidas.";

        if (arquivo.Length > TamanhoMaximoBytes)
            return "A imagem deve ter no máximo 2 MB.";

        return null;
    }

    public static async Task<string> SalvarAsync(IFormFile arquivo, string pastaImagens)
    {
        Directory.CreateDirectory(pastaImagens);

        var nomeArquivo = Guid.NewGuid() + Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        var caminho = Path.Combine(pastaImagens, nomeArquivo);

        using var stream = new FileStream(caminho, FileMode.Create);
        await arquivo.CopyToAsync(stream);

        return nomeArquivo;
    }
}

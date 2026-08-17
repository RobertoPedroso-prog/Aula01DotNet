namespace Aula01.Web.Services
{
    public class MensagemService
    {
        public string ObterMensagem()
        {
            return "Mensagem vinda da camada de serviço";
        }

        public string ObterDetalhesProjeto()
        {
            return "Projeto desenvolvido no ecossistema .NET 8 com ASP.NET Core MVC.";
        }
    }
}
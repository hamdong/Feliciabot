using WaifuSharp;

namespace Feliciabot.services.interfaces
{
    public interface IWaifuSharpService
    {
        public string GetSfwImage(Endpoints.Sfw sfwAction);
    }
}

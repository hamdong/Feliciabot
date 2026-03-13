using Feliciabot.services.interfaces;
using System.Diagnostics.CodeAnalysis;
using WaifuSharp;

namespace Feliciabot.services
{
    [ExcludeFromCodeCoverage]
    public class WaifuSharpService(WaifuClient waifuClient) : IWaifuSharpService
    {
        public string GetSfwImage(Endpoints.Sfw sfwAction)
        {
            return waifuClient.GetSfwImage(sfwAction);
        }
    }
}

using Feliciabot.net._6._0.services.interfaces;
using System.Diagnostics.CodeAnalysis;
using WaifuSharp;

namespace Feliciabot.net._6._0.services
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

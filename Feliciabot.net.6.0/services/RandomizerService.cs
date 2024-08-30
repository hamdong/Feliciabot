using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.services.interfaces;

namespace Feliciabot.net._6._0.services
{
    public class RandomizerService : IRandomizerService
    {
        public int GetRandom(int max, int min = 0)
        {
            return CommandsHelper.GetRandomNumber(max, min);
        }
    }
}

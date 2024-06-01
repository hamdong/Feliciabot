using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Ratquaza.Pokedex2;

namespace Feliciabot.net._6._0.commands
{
    public class PokemonCommand : ModuleBase
    {
        private const int MAX_POKEMON_NUM = 898;

        /// <summary>
        /// Gets specified pokemon pokedex entry and posts to channel
        /// </summary>
        /// <param name="pokeQuery">query to pass into pokedex search</param>
        [Alias("dex")]
        [Command("pokedex", RunMode = RunMode.Async)]
        [Summary("Gets specified pokemon pokedex entry and posts to channel. [Usage]: !pokedex [pikachu]")]
        public async Task PostPokeInfo(string pokeQuery)
        {
            var embed = GetPokeEmbedBuilder(pokeQuery);
            if (embed.Length == 0)
            {
                await Context.Channel.SendMessageAsync("Pokemon numbered/named '" + pokeQuery + "' not found :confused:");
                return;
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        /// <summary>
        /// Gets specified pokemon pokedex entry and posts to channel
        /// </summary>
        [Alias("dex")]
        [Command("pokedex", RunMode = RunMode.Async)]
        [Summary("Gets specified pokemon pokedex entry and posts to channel. [Usage]: !pokedex [pikachu]")]
        public async Task PostPokeInfo()
        {
            int pokeNum = CommandsHelper.GetRandomNumber(MAX_POKEMON_NUM);
            var embed = GetPokeEmbedBuilder(pokeNum.ToString());
            if (embed.Length == 0)
            {
                await Context.Channel.SendMessageAsync("Pokemon numbered/named '" + pokeNum + "' not found :confused:");
                return;
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        /// <summary>
        /// Creates an embed from a specified pokemon name or number
        /// Does not actually build the embed
        /// </summary>
        /// <param name="pokeNameOrNum">pokemon name or number to query</param>
        private static EmbedBuilder GetPokeEmbedBuilder(string pokeNameOrNum)
        {
            Pokemon mon;
            var builder = new EmbedBuilder();

            try
            {
                mon = Pokedex.ByName(pokeNameOrNum);
            }
            catch
            {
                return builder;
            }

            if (mon == null)
                return builder;

            // Build embed and post to channel
            builder.WithTitle("#" + mon.ID + " " + mon.Name);
            builder.AddField("Type1", mon.Types[0]);
            if (mon.Types[0] != mon.Types[1]) builder.AddField("Type2", mon.Types[1]);
            builder.AddField("Generation", mon.Generation);
            builder.WithThumbnailUrl(mon.GetSprite());
            builder.WithFooter("https://pokeapi.co/api/v2/pokemon/" + mon.Name.ToLower());
            return builder;
        }
    }
}

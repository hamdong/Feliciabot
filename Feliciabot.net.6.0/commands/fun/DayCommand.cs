using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;

namespace Feliciabot.net._6._0.commands
{
    public class DayCommand : ModuleBase
    {
        readonly (string videoLink, string title)[] dayEvent;

        public DayCommand()
        {
            string basePath = Environment.CurrentDirectory;

            dayEvent =
            [
                (Path.Combine(basePath, "videos", "sunday.mov"), "It's Energy Sword Sunday!"),
                (
                    Path.Combine(basePath, "videos", "monado.mp4"),
                    "It is currently Monado Monday my dude!"
                ),
                (Path.Combine(basePath, "img", "towa.png"), "It's Towa Tuesday!"),
                (
                    Path.Combine(basePath, "videos", "fish_gaming.mov"),
                    "It's Fish Gaming Wednesday fellow youths!"
                ),
                ("", "It's Out of Touch Thursday!  https://www.youtube.com/watch?v=IneQHuAW3q8"),
                (
                    Path.Combine(basePath, "img", "felicia_img", "felicia_stare.gif"),
                    "It's Felicia Friday!"
                ),
                (Path.Combine(basePath, "img", "radical.png"), "You have entered Radical Saturday"),
                ("", "If you're seeing this message that means we've fallen out of the timeline"),
            ];
        }

        [Command("day", RunMode = RunMode.Async)]
        [Alias("today")]
        [Summary("Posts a meme related to the current day or time")]
        public async Task Day()
        {
            DayOfWeek currentDay = CommandsHelper.GetCurrentTimeEastern().DayOfWeek;

            if (string.IsNullOrEmpty(dayEvent[(int)currentDay].videoLink))
            {
                await Context.Channel.SendMessageAsync(dayEvent[(int)currentDay].title);
                return;
            }

            await Context.Channel.SendFileAsync(
                dayEvent[(int)currentDay].videoLink,
                dayEvent[(int)currentDay].title
            );
        }
    }
}

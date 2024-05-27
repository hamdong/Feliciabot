using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    public class DayCommand : ModuleBase
    {
        readonly (string videoLink, string title)[] dayEvent = [
            (Environment.CurrentDirectory + @"\videos\sunday.mov", "It's Energy Sword Sunday!"),
            (Environment.CurrentDirectory + @"\videos\monado.mp4", "It is currently Monado Monday my dude!"),
            (Environment.CurrentDirectory + @"\img\towa.png", "It's Towa Tuesday!"),
            (Environment.CurrentDirectory + @"\videos\fish_gaming.mov", "It's Fish Gaming Wednesday fellow youths!"),
            ("","It's Out of Touch Thursday!  https://www.youtube.com/watch?v=IneQHuAW3q8"),
            (Environment.CurrentDirectory + @"\img\felicia_img\felicia_stare.gif", "It's Felicia Friday!"),
            (Environment.CurrentDirectory + @"\img\radical.png", "You have entered Radical Saturday"),
            ("","If you're seeing this message that means we've fallen out of the timeline")
        ];

        [Command("day", RunMode = RunMode.Async)]
        [Alias("today")]
        [Summary("Posts a meme related to the current day or time")]
        public async Task Day()
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DayOfWeek currentDay = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, estZone).DayOfWeek;

            if (dayEvent[(int)currentDay].videoLink == string.Empty)
            {
                await Context.Channel.SendMessageAsync(dayEvent[(int)currentDay].title);
                return;
            }

            await Context.Channel.SendFileAsync(dayEvent[(int)currentDay].videoLink, dayEvent[(int)currentDay].title);
        }
    }
}

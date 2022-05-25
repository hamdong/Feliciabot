using Discord.Commands;

namespace Feliciabot.net._6._0.commands
{
    /// <summary>
    /// Commands pertaining to posting Day memes
    /// </summary>
    public class DayCommand : ModuleBase
    {
        /// <summary>
        /// Posts a meme relative to the current day or time
        /// </summary>
        /// <returns>Task containing the response from the day command</returns>
        [Command("day", RunMode = RunMode.Async)]
        [Alias("today")]
        [Summary("Posts a meme related to the current day or time. [Usage] !day, !today")]
        public async Task Day()
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DayOfWeek currentDay = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, estZone).DayOfWeek;
            switch (currentDay.ToString())
            {
                case "Monday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\monado.mp4", "It is currently Monado Monday my dude!");
                    break;
                case "Tuesday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\img\towa.png", "It's Towa Tuesday!");
                    break;
                case "Wednesday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\fish_gaming.mov", "It's Fish Gaming Wednesday fellow youths!");
                    break;
                case "Thursday":
                    await Context.Channel.SendMessageAsync("It's Out of Touch Thursday!  https://www.youtube.com/watch?v=IneQHuAW3q8");
                    break;
                case "Friday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\img\felicia_img\felicia_stare.gif", "It's Felicia Friday!");
                    break;
                case "Saturday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\img\radical.png", "You have entered Radical Saturday");
                    break;
                case "Sunday":
                    await Context.Channel.SendFileAsync(Environment.CurrentDirectory + @"\videos\sunday.mov", "It's Energy Sword Sunday!");
                    break;
                default:
                    await Context.Channel.SendMessageAsync($"If you're seeing this message that means we've fallen out of the timeline");
                    break;
            }
        }
    }
}

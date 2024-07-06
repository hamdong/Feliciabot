using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Feliciabot.net._6._0.services
{
    public sealed class BirthdayService(DiscordSocketClient _client) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = CommandsHelper.GetCurrentTimeEastern();
                var nextMidnight = CalculateNextMidnight(now);

                if (now >= nextMidnight)
                {
                    nextMidnight = nextMidnight.AddDays(1);
                }

                var delay = nextMidnight - now;
                await Task.Delay(delay, stoppingToken); // for testing, change delay to TimeSpan.FromSeconds(10)
                await CheckForBirthdaysAtTime(now);
            } while (!stoppingToken.IsCancellationRequested);
        }

        private async Task CheckForBirthdaysAtTime(DateTime now)
        {
            var guildIds = _client.Guilds.Select(g => g.Id).ToList();
            var userGuildToBdays = LoadBirthdays(guildIds).Where(x => IsBirthday(now, x.Value)).ToList();

            try
            {
                foreach (var userGuildToBday in userGuildToBdays)
                {
                    var userId = userGuildToBday.Key.Split('-')[0];
                    var guildId = userGuildToBday.Key.Split('-')[1];
                    var guild = _client.GetGuild(Convert.ToUInt64(guildId));

                    if (guild == null) continue;

                    var channel = CommandsHelper.GetGeneralChannelFromGuild(guild);
                    var user = guild.Users.FirstOrDefault(x => x.Id.ToString() == userId);

                    if (user != null && channel != null)
                    {
                        await SendBirthdayMessageToChannel(channel, user);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Log($"Error checking birthdays with exception: {e.Message}");
            }
        }

        private static bool IsBirthday(DateTime date, string formattedBirthday)
        {
            var parts = formattedBirthday.Split('-');
            int month = int.Parse(parts[0]);
            int day = int.Parse(parts[1]);

            return date.Day == day && date.Month == month;
        }

        private static Dictionary<string, string> LoadBirthdays(List<ulong> guildIds)
        {
            if (!File.Exists("data/birthdays.json"))
            {
                return [];
            }

            var birthdaysJson = File.ReadAllText(Environment.CurrentDirectory + @"\data\birthdays.json");
            var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(birthdaysJson) ?? [];
            var filteredEntries = entries.Where(entry => guildIds.Exists(id => entry.Key.Contains(id.ToString()))).ToList();

            return filteredEntries.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private static async Task SendBirthdayMessageToChannel(SocketTextChannel channel, SocketGuildUser user)
        {
            await channel.SendMessageAsync($"Happy birthday, {user.Mention}");
        }

        private static DateTime CalculateNextMidnight(DateTime currentTime)
        {
            // Determine if daylight saving time is in effect
            bool isDst = ((currentTime.Kind == DateTimeKind.Utc && currentTime.TimeOfDay.Hours <= 4) ||
                         (currentTime.Kind == DateTimeKind.Local && currentTime.TimeOfDay.Hours <= 5));

            // Calculate the next midnight, adjusting for daylight saving time
            DateTime nextMidnight = currentTime.Date.AddHours(isDst ? 24 - currentTime.TimeOfDay.Hours : 24);

            return nextMidnight;
        }
    }
}

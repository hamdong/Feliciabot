using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using System.Text.Json;

namespace Feliciabot.net._6._0.services
{
    public sealed class BirthdayService(DiscordSocketClient _client) : IDisposable
    {
        private Timer? timer;

        public void ResetTimer()
        {
            timer = new Timer(CheckBirthdays, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        }

        private async void CheckBirthdays(object? state)
        {
            var now = CommandsHelper.GetCurrentTimeEastern();
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

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}

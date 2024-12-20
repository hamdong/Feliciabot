﻿using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Feliciabot.net._6._0.helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Feliciabot.net._6._0.services
{
    public sealed class BirthdayService(
        DiscordSocketClient _client,
        ILogger<BirthdayService> _logger
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.UtcNow;
                var nextMidnightOffset = CalculateNextMidnight(now);
                var timeToCheck = now + nextMidnightOffset;

                await Task.Delay(nextMidnightOffset, stoppingToken);
                await CheckForBirthdaysAtTime(timeToCheck);
            } while (!stoppingToken.IsCancellationRequested);
        }

        private async Task CheckForBirthdaysAtTime(DateTime now)
        {
            var guildIds = _client.Guilds.Select(g => g.Id).ToList();
            var userGuildToBdays = LoadBirthdays(guildIds)
                .Where(x => IsBirthday(now, x.Value))
                .ToList();

            try
            {
                foreach (var userGuildToBday in userGuildToBdays)
                {
                    var userId = userGuildToBday.Key.Split('-')[0];
                    var guildId = userGuildToBday.Key.Split('-')[1];
                    var guild = _client.GetGuild(Convert.ToUInt64(guildId));

                    if (guild == null)
                        continue;

                    var channel = await CommandsHelper.GetGeneralChannelFromGuildAsync(guild);
                    var user = guild.Users.FirstOrDefault(x => x.Id.ToString() == userId);

                    if (user != null && channel != null)
                    {
                        await SendBirthdayMessageToChannel(channel, user);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error checking birthdays with exception: {Message}", e.Message);
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

            var birthdaysJson = File.ReadAllText(
                Environment.CurrentDirectory + @"\data\birthdays.json"
            );
            var entries =
                JsonSerializer.Deserialize<Dictionary<string, string>>(birthdaysJson) ?? [];
            var filteredEntries = entries
                .Where(entry => guildIds.Exists(id => entry.Key.Contains(id.ToString())))
                .ToList();

            return filteredEntries.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private static async Task SendBirthdayMessageToChannel(
            IMessageChannel channel,
            SocketGuildUser user
        )
        {
            await channel.SendMessageAsync($"Happy birthday, {user.Mention}");
        }

        private static TimeSpan CalculateNextMidnight(DateTime utcCurrentTime)
        {
            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(utcCurrentTime, estTimeZone);

            // Calculate the next midnight in Eastern Time
            DateTime nextMidnight = localNow.Date.AddHours(localNow.TimeOfDay.Hours > 0 ? 24 : 0);

            // Calculate the duration until the next midnight
            return nextMidnight - localNow;
        }
    }
}

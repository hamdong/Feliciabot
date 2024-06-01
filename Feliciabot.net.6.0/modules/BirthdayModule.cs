using Discord.Interactions;
using System.Globalization;
using System.Text.Json;

namespace Feliciabot.net._6._0.modules
{
    public sealed class BirthdayModule : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly string birthdayPath = Environment.CurrentDirectory + @"\data\birthdays.json";

        [SlashCommand("birthday", "Set your birthday, input date as 'M/d'", runMode: RunMode.Async)]
        public async Task SetBirthday(string birthday)
        {
            if (!DateTime.TryParseExact(birthday, "M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                await RespondAsync("Invalid birthday format. Please enter your birthday in the format 'M/d', e.g., '4/20'.").ConfigureAwait(false);
                return;
            }

            string formattedBirthday = parsedDate.ToString("M/d");
            ulong guildId = Context.Guild?.Id ?? 0;

            if (await SaveBirthdayAsync(Context.User.Id, guildId, formattedBirthday))
            {
                await RespondAsync("Your birthday has been saved!").ConfigureAwait(false);
            }
            else
            {
                await RespondAsync("Unable to save birthday. Try again?").ConfigureAwait(false);
            }
        }

        private static async Task<bool> SaveBirthdayAsync(ulong userId, ulong guildId, string formattedBirthday)
        {
            try
            {
                var birthdays = LoadBirthdays(birthdayPath);
                birthdays[$"{userId}-{guildId}1"] = formattedBirthday;
                await File.WriteAllTextAsync(birthdayPath, JsonSerializer.Serialize(birthdays));
            }
            catch (Exception ex)
            {
                LogHelper.Log($"An error occurred while saving the birthday: {ex.Message}");
                return false;
            }

            return true;
        }

        private static Dictionary<string, string> LoadBirthdays(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                return [];
            }

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
            }
            catch (Exception)
            {
                return [];
            }

        }
    }
}

using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.helpers;
using Feliciabot.net._6._0.models;
using Newtonsoft.Json;
using System.Text;
using UwUfyer;

namespace Feliciabot.net._6._0.commands
{
    public class UwuCommand : ModuleBase
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor for Uwu command
        /// </summary>
        /// <param name="httpClient">client service to use</param>
        public UwuCommand(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Posts uwuified message of the previous user's message
        /// </summary>
        /// <returns>Task containing the uwuified message</returns>
        [Command("uwu", RunMode = RunMode.Async)]
        [Alias("uwufy", "uwuify")]
        [Summary("Posts uwuified message. [Usage] !uwu, !uwufy, !uwuify")]
        public async Task Uwu()
        {
            // Get last batch of messages
            IEnumerable<IMessage> messages;
            messages = await Context.Channel.GetMessagesAsync(50).FlattenAsync();

            // Get the last message from the user who posted
            string foundMessage = "No messages found uwu.";
            foreach (IMessage m in messages)
            {
                if (!m.Author.IsBot && CommandsHelper.IsNonCommandQuery(m.Content) && !CommandsHelper.IsEmoteMessage(m.Content))
                {
                    foundMessage = m.Content;
                    break;
                }
            }

            // Call out
            string response = await GetUwuRequest(foundMessage);
            await Context.Channel.SendMessageAsync(response);
        }

        /// <summary>
        /// Overloaded method, posts uwuified message of the specified message
        /// </summary>
        /// <param name="message">Message to uwuify</param>
        /// <returns>Task containing the uwuified message</returns>
        [Command("uwu", RunMode = RunMode.Async)]
        [Alias("uwufy", "uwuify")]
        [Summary("Posts uwuified message. [Usage] !uwu [Sample text], !uwufy, !uwuify")]
        public async Task Uwu([Remainder] string message)
        {
            string response = await GetUwuRequest(message);
            await Context.Channel.SendMessageAsync(response);
        }

        /// <summary>
        /// Makes a call to Uwuifier Node app on localhost to get an uwuified string
        /// </summary>
        /// <param name="text">Message to uwuify</param>
        /// <returns>The message as an uwuified string</returns>
        public async Task<string> GetUwuRequest(string text)
        {
            // Create object to be serialized
            UwuText uwu = new UwuText
            {
                text = text
            };

            // Create message body
            string json = JsonConvert.SerializeObject(uwu);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8080"),
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            // Uwuify text
            string responseBody;
            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // If unable to use Node app just use this nuget package Uwufy
                responseBody = text.Uwufy();
            }

            // Remove the backticks
            responseBody = responseBody.Replace("`", "\\`");
            return responseBody;
        }
    }
}

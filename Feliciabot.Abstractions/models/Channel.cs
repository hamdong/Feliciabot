using Discord;
using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;

namespace Feliciabot.Abstractions.models
{
    public class Channel : IChannelFactory
    {
        private IMessageChannel? MessageChannel { get; set; }
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong GuildId { get; set; }
        public string GuildName { get; set; }


        public Channel()
        {
            Id = 0;
            Name = "channel";
            GuildId = 0;
            GuildName = "guild";
        }

        public Channel(ulong id, string name, ulong guildId, string guildName)
        {
            Id = id;
            Name = name;
            GuildId = guildId;
            GuildName = guildName;
        }

        public Channel(SocketGuildChannel socketGuildChannel)
        {
            Id = socketGuildChannel.Id;
            Name = socketGuildChannel.Name;
            GuildId = socketGuildChannel.Guild.Id;
            GuildName = socketGuildChannel.Guild.Name;
            MessageChannel = socketGuildChannel as IMessageChannel;
        }

        public virtual async Task SendMessageToChannelAsync(string message)
        {
            if (MessageChannel != null) await MessageChannel.SendMessageAsync(message);
        }

        public static Channel[] FromSocketGuildChannels(IReadOnlyCollection<SocketGuildChannel> socketGuildChannels)
        {
            return socketGuildChannels.Select(socketGuildChannel => new Channel(socketGuildChannel)).ToArray();
        }

        public static Channel FromSocketGuildChannel(SocketGuildChannel socketGuildChannel)
        {
            return new Channel(socketGuildChannel);
        }
    }
}

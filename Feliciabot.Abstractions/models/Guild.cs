﻿using Discord.WebSocket;

namespace Feliciabot.Abstractions.models
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public virtual Role[] Roles { get; set; }
        public virtual Channel[] Channels { get; set; }
        public virtual User[] Users { get; set; }

        public Guild()
        {
            Id = 0;
            Name = "guild";
            Roles = [];
            Channels = [];
            Users = [];
        }

        public Guild(ulong id, string name)
        {
            Id = id;
            Name = name;
            Roles = [];
            Channels = [];
            Users = [];
        }

        public Guild(ulong id, string name, User user)
        {
            Id = id;
            Name = name;
            Roles = [];
            Channels = [];
            Users = [user];
        }

        public Guild(SocketGuild socketGuild)
        {
            Id = socketGuild.Id;
            Name = socketGuild.Name;
            Roles = Role.FromSocketRoles(socketGuild.Roles);
            Channels = Channel.FromSocketGuildChannels(socketGuild.Channels);
            Users = User.FromSocketGuildUsers(socketGuild.Users);
        }

        public static Guild FromSocketGuild(SocketGuild socketGuild)
        {
            return new Guild(socketGuild);
        }
    }
}

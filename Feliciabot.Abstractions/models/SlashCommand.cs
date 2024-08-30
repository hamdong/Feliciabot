using Discord.Interactions;

namespace Feliciabot.Abstractions.models
{
    public class SlashCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }

        public SlashCommand(string name, string description, string moduleName)
        {
            Name = name;
            Description = description;
            ModuleName = moduleName;
        }

        public static SlashCommand FromSlashCommandInfo(SlashCommandInfo info)
        {
            return new SlashCommand(info.Name, info.Description, info.Module.Name);
        }
    }
}

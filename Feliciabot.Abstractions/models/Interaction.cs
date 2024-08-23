using Discord.Interactions;

namespace Feliciabot.Abstractions.models
{
    public class Interaction
    {
        public IReadOnlyList<SlashCommand> SlashCommands { get; set; }
        public Interaction(IReadOnlyList<SlashCommand> commands)
        {
            SlashCommands = commands;
        }

        public static Interaction FromSlashCommandList(IReadOnlyList<SlashCommandInfo> infos)
        {
            List<SlashCommand> slashCommands = [];
            foreach (var i in infos)
            {
                slashCommands.Add(new SlashCommand(i.Name, i.Description, i.Module.Name));
            }
            return new Interaction(slashCommands.AsReadOnly());
        }
    }
}

using Discord;
using Discord.Commands;
using Discord.Interactions;
using System.Reflection;

namespace FeliciabotTests.tests
{
    public static class TestCommandContext
    {
        public static void SetContext(ModuleBase<ICommandContext> module, ICommandContext commandContext)
        {
            var setContext = module?.GetType().GetMethod("Discord.Commands.IModuleBase.SetContext", BindingFlags.NonPublic | BindingFlags.Instance);
            setContext?.Invoke(module, [commandContext]);
        }

        public static void SetContext(InteractionModuleBase<SocketInteractionContext> module, IInteractionContext commandContext)
        {
            var setContext = module?.GetType().GetMethod("Discord.Commands.IModuleBase.SetContext", BindingFlags.NonPublic | BindingFlags.Instance);
            setContext?.Invoke(module, [commandContext]);
        }
    }
}

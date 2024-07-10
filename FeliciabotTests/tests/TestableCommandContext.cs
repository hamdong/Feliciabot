using Discord.Commands;
using System.Reflection;

namespace FeliciabotTests.tests
{
    public static class TestCommandContext
    {
        public static void SetContext(ModuleBase<ICommandContext> module, ICommandContext commandContext)
        {
            var setContext = module.GetType().GetMethod("Discord.Commands.IModuleBase.SetContext", BindingFlags.NonPublic | BindingFlags.Instance);
            setContext?.Invoke(module, [commandContext]);
        }
    }
}

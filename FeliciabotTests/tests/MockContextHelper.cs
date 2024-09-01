using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Feliciabot.net._6._0.modules;
using Moq;
using WaifuSharp;

namespace FeliciabotTests.tests
{
    public static class MockContextHelper
    {
        public static void SetContext(InteractionModuleBase module, IInteractionContext context)
        {
            var fieldInfo = typeof(InteractionModuleBase<IInteractionContext>)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "set_Context")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments(),
                })
                .Select(x => x.Method)
                .First();
            fieldInfo?.Invoke(module, [context]);
        }

        public static void SetContext(ModuleBase module, ICommandContext context)
        {
            var fieldInfo = typeof(ModuleBase<ICommandContext>)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "set_Context")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments(),
                })
                .Select(x => x.Method)
                .First();
            fieldInfo?.Invoke(module, [context]);
        }
    }
}

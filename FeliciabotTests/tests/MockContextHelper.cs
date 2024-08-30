using System.Linq.Expressions;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Moq;

namespace FeliciabotTests.tests
{
    public static class MockContextHelper
    {
        public static void SetContext(
            ModuleBase<ICommandContext> module,
            ICommandContext commandContext
        )
        {
            var setContext = module
                ?.GetType()
                .GetMethod(
                    "Discord.Commands.IModuleBase.SetContext",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
            setContext?.Invoke(module, [commandContext]);
        }

        public static void SetContext(
            InteractionModuleBase<SocketInteractionContext> module,
            IInteractionContext commandContext
        )
        {
            var setContext = module
                ?.GetType()
                .GetMethod(
                    "Discord.Commands.IModuleBase.SetContext",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
            setContext?.Invoke(module, [commandContext]);
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

        public static void VerifyMessageSentAsync(
            Mock<IMessageChannel> channel,
            Func<string, bool> msgPredicate
        )
        {
            Expression<Func<string, bool>> expr = msg => msgPredicate(msg);
            channel.Verify(
                c =>
                    c.SendMessageAsync(
                        It.Is(expr),
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        MessageFlags.None,
                        null
                    ),
                Times.Once
            );
        }
    }
}

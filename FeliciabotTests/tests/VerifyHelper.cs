using System.Linq.Expressions;
using Discord;
using Moq;

namespace FeliciabotTests.tests
{
    public static class VerifyHelper
    {
        public static void VerifyMessageSentAsync(
            Mock<IMessageChannel> channel,
            Func<string, bool> msgPredicate,
            int times = 1
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
                Times.Exactly(times)
            );
        }

        public static void VerifyMessageSentAsync(
            Mock<ITextChannel> channel,
            Func<string, bool> msgPredicate,
            int times = 1
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
                Times.Exactly(times)
            );
        }

        public static void VerifyNoMessageSentAsync(Mock<IMessageChannel> channel)
        {
            channel.Verify(
                c =>
                    c.SendMessageAsync(
                        It.IsAny<string>(),
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
                Times.Never
            );
        }

        public static void VerifyNoMessageSentAsync(Mock<ITextChannel> channel)
        {
            channel.Verify(
                c =>
                    c.SendMessageAsync(
                        It.IsAny<string>(),
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
                Times.Never
            );
        }

        public static void VerifyFileSentAsync(
            Mock<IMessageChannel> channel,
            Func<string, bool> msgPredicate,
            int times = 1
        )
        {
            Expression<Func<string, bool>> expr = msg => msgPredicate(msg);
            channel.Verify(
                c =>
                    c.SendFileAsync(
                        It.Is(expr),
                        null,
                        false,
                        null,
                        null,
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        MessageFlags.None,
                        null
                    ),
                Times.Exactly(times)
            );
        }

        public static void VerifyInteractionAsync(
            Mock<IInteractionContext> context,
            Func<string, bool> msgPredicate,
            int times = 1
        )
        {
            Expression<Func<string, bool>> expr = msg => msgPredicate(msg);
            context.Verify(
                c =>
                    c.Interaction.RespondAsync(
                        It.Is(expr),
                        null,
                        false,
                        false,
                        null,
                        null,
                        It.IsAny<Embed>(),
                        null,
                        null
                    ),
                Times.Exactly(times)
            );
        }
    }
}

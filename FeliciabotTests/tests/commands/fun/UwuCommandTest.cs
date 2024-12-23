using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class UwuCommandTest
    {
        private readonly Mock<IMessageChannel> mockChannel;
        private readonly Mock<ICommandContext> mockContext;
        private readonly UwuCommand uwuCommand;

        public UwuCommandTest()
        {
            mockChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            uwuCommand = new UwuCommand();
        }

        [SetUp]
        public void Setup()
        {
            mockContext.SetupGet(c => c.Channel).Returns(mockChannel.Object);
            MockContextHelper.SetContext(uwuCommand, mockContext.Object);
        }

        [Test]
        public async Task Uwu_WithMessage_ShouldUwuifyMessage()
        {
            await uwuCommand.Uwu("Hello world");

            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Count(c => char.ToLowerInvariant(c) == 'w') >= 5
            );
        }
    }
}

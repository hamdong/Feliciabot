using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class TroubleCommandTest
    {
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IMessagingService> _mockMessagingService;
        private readonly TroubleCommand _troubleCommand;
        public TroubleCommandTest()
        {
            _mockContext = new Mock<ICommandContext>();
            _mockMessagingService = new Mock<IMessagingService>();
            _troubleCommand = new TroubleCommand(_mockMessagingService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockMessagingService.Reset();
        }

        [Test]
        public async Task Trouble_MessagesThreeTimes()
        {
            _mockMessagingService.Setup(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            TestCommandContext.SetContext(_troubleCommand, _mockContext.Object);

            await _troubleCommand.Trouble();

            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public async Task Chairman_MessagesOnce()
        {
            _mockMessagingService.Setup(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            TestCommandContext.SetContext(_troubleCommand, _mockContext.Object);

            await _troubleCommand.Chairman();

            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>()), Times.Once);
        }
    }
}

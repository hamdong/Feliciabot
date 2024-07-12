using Discord.Commands;
using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class TroubleCommandTest
    {
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<DiscordSocketClient> _mockClient;
        private readonly Mock<IGuildFactory> _mockGuildFactory;
        private readonly Mock<ClientService> _mockClientService;
        private readonly Mock<MessagingService> _mockMessagingService;
        private readonly TroubleCommand _troubleCommand;
        public TroubleCommandTest()
        {
            _mockContext = new Mock<ICommandContext>();
            _mockClient = new Mock<DiscordSocketClient>();
            _mockGuildFactory = new Mock<IGuildFactory>();
            _mockClientService = new Mock<ClientService>(_mockClient.Object, _mockGuildFactory.Object);
            _mockMessagingService = new Mock<MessagingService>(_mockClientService.Object);
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

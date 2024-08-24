using Discord;
using Discord.Commands;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class MessagingServiceTest
    {
        private readonly Mock<IGuild> _mockGuild;
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IGuildService> _mockGuildService;
        private readonly MessagingService _messagingService;

        public MessagingServiceTest()
        {
            _mockGuild = new Mock<IGuild>();
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _mockGuildService = new Mock<IGuildService>();
            _messagingService = new MessagingService(_mockGuildService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockGuild.SetupGet(g => g.Id).Returns(9999);
            _mockChannel.SetupGet(c => c.Id).Returns(2222);
            _mockContext.SetupGet(c => c.Guild).Returns(_mockGuild.Object);
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
        }

        [Test]
        public async Task SendMessageToContextAsync_WithValidChannel_SendsMessage()
        {
            Mock<Channel> expectedChannel = new(_mockChannel.Object.Id, "Channel", _mockGuild.Object.Id, "Guild");
            _mockGuildService.Setup(s => s.GetChannelByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns(expectedChannel.Object);

            await _messagingService.SendMessageToContextAsync(_mockContext.Object, "Test");

            expectedChannel.Verify(c => c.SendMessageToChannelAsync("Test"), Times.Once);
        }

        [Test]
        public async Task SendMessageToContextAsync_WithInValidChannel_Cancels()
        {
            Mock<Channel> expectedChannel = new();
            _mockGuildService.Setup(s => s.GetChannelByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns(null as Channel);

            await _messagingService.SendMessageToContextAsync(_mockContext.Object, "Test");

            expectedChannel.Verify(c => c.SendMessageToChannelAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task SendMessageToChannelByIdAsync_WithValidChannel_SendsMessage()
        {
            Mock<Channel> expectedChannel = new(_mockChannel.Object.Id, "Channel", _mockGuild.Object.Id, "Guild");
            _mockGuildService.Setup(s => s.GetChannelByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns(expectedChannel.Object);

            await _messagingService.SendMessageToChannelByIdAsync(_mockGuild.Object.Id, _mockChannel.Object.Id, "Test");

            expectedChannel.Verify(c => c.SendMessageToChannelAsync("Test"), Times.Once);
        }

        [Test]
        public async Task SendMessageToChannelByIdAsync_WithInValidChannel_Cancels()
        {
            Mock<Channel> expectedChannel = new();
            _mockGuildService.Setup(s => s.GetChannelByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns(null as Channel);

            await _messagingService.SendMessageToChannelByIdAsync(_mockGuild.Object.Id, _mockChannel.Object.Id, "Test");

            expectedChannel.Verify(c => c.SendMessageToChannelAsync(It.IsAny<string>()), Times.Never);
        }
    }
}

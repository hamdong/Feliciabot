using Discord;
using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    public class ClientServiceTest
    {
        private readonly Mock<DiscordSocketClient> _mockDiscordClient;
        private readonly Mock<IClientFactory> _mockClientFactory;
        private readonly ClientService _clientService;

        public ClientServiceTest()
        {
            _mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockClientFactory = new Mock<IClientFactory>();
            _clientService = new ClientService(_mockDiscordClient.Object, _mockClientFactory.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockClientFactory.Reset();
        }

        [Test]
        public void GetClient_ReturnsClient()
        {
            var mockActivity = new Mock<IActivity>();
            mockActivity.Setup(a => a.Name).Returns("Activity");
            Client expectedClient = new("client", UserStatus.Online, mockActivity.Object);
            _mockClientFactory.Setup(c => c.FromDiscordSocketClient(It.IsAny<DiscordSocketClient>())).Returns(expectedClient);

            var result = _clientService.GetClient();

            _mockClientFactory.Verify(g => g.FromDiscordSocketClient(It.IsAny<DiscordSocketClient>()), Times.Once);
            Assert.That(expectedClient.Username, Is.EqualTo(result.Username));
            Assert.That(expectedClient.Status, Is.EqualTo(result.Status));
            Assert.That(expectedClient.Activities, Is.EqualTo(result.Activities));
            Assert.That(expectedClient.Guilds, Is.EqualTo(result.Guilds));
        }
    }
}

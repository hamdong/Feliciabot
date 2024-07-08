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
        private readonly Mock<IGuildFactory> _mockGuildFactory;
        private readonly ClientService _clientService;

        public ClientServiceTest()
        {
            _mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockGuildFactory = new Mock<IGuildFactory>();
            _clientService = new ClientService(_mockDiscordClient.Object, _mockGuildFactory.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockGuildFactory.Reset();
        }

        [Test]
        public void GetGuildById_ReturnsGuild()
        {
            Guild expectedGuild = new(0, "guild");
            _mockGuildFactory.Setup(g => g.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            var result = _clientService.GetGuildById(expectedGuild.Id);

            _mockGuildFactory.Verify(g => g.FromSocketGuild(It.IsAny<SocketGuild>()), Times.Once);
            Assert.That(expectedGuild.Id, Is.EqualTo(result?.Id));
        }

        [Test]
        public void GetUserByGuildById_WithFoundUser_ReturnsUser()
        {
            User expectedUser = new(0, "user", "0000");
            Guild expectedGuild = new(0, "guild", expectedUser);
            _mockGuildFactory.Setup(u => u.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            var result = _clientService.GetUserByGuildById(It.IsAny<ulong>(), expectedUser.Id);

            _mockGuildFactory.Verify(u => u.FromSocketGuild(It.IsAny<SocketGuild>()), Times.Once);
            Assert.That(expectedUser.Id, Is.EqualTo(result?.Id));
        }
    }
}

using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class GuildServiceTest
    {
        private readonly Mock<DiscordSocketClient> _mockDiscordClient;
        private readonly Mock<User> _mockUser;
        private readonly Mock<IGuildFactory> _mockGuildFactory;
        private readonly GuildService _guildService;

        private const ulong expectedGuildId = 9999;
        private const ulong expectedChannelId = 2222;
        private readonly Channel[] expectedChannel = [new(expectedChannelId, "channel", expectedGuildId, "guild")];
        private readonly Role expectedRole = new(1111, "trouble", expectedGuildId, "guild");

        public GuildServiceTest()
        {
            _mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockUser = new Mock<User>();
            _mockGuildFactory = new Mock<IGuildFactory>();
            _guildService = new GuildService(_mockDiscordClient.Object, _mockGuildFactory.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockUser.Reset();
            _mockGuildFactory.Reset();
        }

        [Test]
        public void GetChannelByGuildById_WithFoundChannel_ReturnsChannel()
        {
            Guild expectedGuild = new(0, "guild")
            {
                Channels = expectedChannel
            };
            _mockGuildFactory.Setup(u => u.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            var result = _guildService.GetChannelByGuildById(It.IsAny<ulong>(), expectedChannel[0].Id);

            _mockGuildFactory.Verify(u => u.FromSocketGuild(It.IsAny<SocketGuild>()), Times.Once);
            Assert.That(expectedChannel[0].Id, Is.EqualTo(result?.Id));
        }

        [Test]
        public async Task AddRoleToUserByIdAsync_WithFoundUser_AddsRoleToUser()
        {
            _mockUser.Setup(u => u.AddRoleByIdAsync(It.IsAny<ulong>())).Returns(Task.CompletedTask);
            Guild expectedGuild = new(expectedGuildId, "guild")
            {
                Channels = expectedChannel,
                Roles = [expectedRole],
                Users = [_mockUser.Object]
            };
            _mockGuildFactory.Setup(s => s.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            await _guildService.AddRoleToUserByIdAsync(expectedGuild.Id, _mockUser.Object.Id, expectedRole.Id);

            _mockUser.Verify(u => u.AddRoleByIdAsync(expectedRole.Id), Times.Once);
        }

        [Test]
        public async Task AddRoleToUserByIdAsync_WithNoFoundUser_DoesNotAddRoleToUser()
        {
            Guild expectedGuild = new(expectedGuildId, "guild")
            {
                Channels = expectedChannel,
                Roles = [expectedRole]
            };
            _mockGuildFactory.Setup(s => s.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            await _guildService.AddRoleToUserByIdAsync(It.IsAny<ulong>(), It.IsAny<ulong>(), It.IsAny<ulong>());

            _mockUser.Verify(u => u.AddRoleByIdAsync(It.IsAny<ulong>()), Times.Never);
        }

        [Test]
        public void GetRoleIdByName_WithFoundRole_ReturnsCorrectRoleId()
        {
            Guild expectedGuild = new(expectedGuildId, "guild")
            {
                Channels = expectedChannel,
                Roles = [expectedRole]
            };
            _mockGuildFactory.Setup(s => s.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            ulong actualRoleId = _guildService.GetRoleIdByName(expectedGuild.Id, "trouble");

            Assert.That(actualRoleId, Is.EqualTo(expectedRole.Id));
        }

        [Test]
        public void GetRoleIdByName_WithNoFoundRole_ReturnsZeroId()
        {
            Guild expectedGuild = new(expectedGuildId, "guild")
            {
                Channels = expectedChannel
            };
            _mockGuildFactory.Setup(s => s.FromSocketGuild(It.IsAny<SocketGuild>())).Returns(expectedGuild);

            ulong actualRoleId = _guildService.GetRoleIdByName(expectedGuild.Id, "trouble");

            Assert.That(actualRoleId, Is.EqualTo(0));
        }
    }
}

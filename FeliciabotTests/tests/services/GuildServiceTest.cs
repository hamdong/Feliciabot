using Discord.WebSocket;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class GuildServiceTest
    {
        private readonly ulong expectedGuildId = GenerateRandomUlong();
        private readonly ulong expectedUserId = GenerateRandomUlong();
        private readonly ulong expectedRoleId = GenerateRandomUlong();

        private readonly Mock<DiscordSocketClient> _mockDiscordClient;
        private readonly Mock<Guild> _mockGuild;
        private readonly Mock<User> _mockUser;
        private readonly Mock<ClientService> _mockClientService;
        private readonly GuildService _guildService;

        public GuildServiceTest()
        {
            _mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockGuild = new Mock<Guild>();
            _mockUser = new Mock<User>();
            _mockClientService = new Mock<ClientService>(_mockDiscordClient.Object);
            _guildService = new GuildService(_mockClientService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockUser.Reset();
        }

        [Test]
        public async Task AddRoleToUserByIdAsync_WithFoundUser_AddsRoleToUser()
        {
            _mockClientService.Setup(s => s.GetUserByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns(_mockUser.Object);

            await _guildService.AddRoleToUserByIdAsync(expectedGuildId, expectedUserId, expectedRoleId);

            _mockUser.Verify(u => u.AddRoleByIdAsync(expectedRoleId), Times.Once);
        }

        [Test]
        public async Task AddRoleToUserByIdAsync_WithNoFoundUser_DoesNotAddRoleToUser()
        {
            _mockClientService.Setup(s => s.GetUserByGuildById(It.IsAny<ulong>(), It.IsAny<ulong>())).Returns((User?)null);

            await _guildService.AddRoleToUserByIdAsync(It.IsAny<ulong>(), It.IsAny<ulong>(), It.IsAny<ulong>());

            _mockUser.Verify(u => u.AddRoleByIdAsync(It.IsAny<ulong>()), Times.Never);
        }

        [Test]
        public void GetRoleIdByName_WithFoundRole_ReturnsCorrectRoleId()
        {
            var expectedRole = new Role(expectedRoleId, "trouble", expectedGuildId, "guild");
            _mockGuild.Setup(g => g.Roles).Returns([expectedRole]);
            _mockClientService.Setup(s => s.GetGuildById(expectedGuildId)).Returns(_mockGuild.Object);

            ulong actualRoleId = _guildService.GetRoleIdByName(expectedGuildId, "trouble");

            Assert.That(actualRoleId, Is.EqualTo(expectedRoleId));
        }

        [Test]
        public void GetRoleIdByName_WithNoFoundRole_ReturnsZeroId()
        {
            _mockGuild.Setup(g => g.Roles).Returns([]);
            _mockClientService.Setup(s => s.GetGuildById(expectedGuildId)).Returns(_mockGuild.Object);

            ulong actualRoleId = _guildService.GetRoleIdByName(expectedGuildId, "trouble");

            Assert.That(actualRoleId, Is.EqualTo(0));
        }

        private static ulong GenerateRandomUlong()
        {
            var random = new Random();
            byte[] randomNumberBytes = new byte[8];
            random.NextBytes(randomNumberBytes);
            return BitConverter.ToUInt64(randomNumberBytes, 0);
        }
    }
}

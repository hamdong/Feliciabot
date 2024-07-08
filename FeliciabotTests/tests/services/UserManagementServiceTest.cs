using Discord.WebSocket;
using Feliciabot.Abstractions.interfaces;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class UserManagementServiceTest
    {
        private readonly UserManagementService _userManagementService;
        private readonly Mock<IGuildFactory> _mockGuildFactory;
        private readonly Mock<ClientService> _mockClientService;
        private readonly Mock<GuildService> _mockGuildService;

        private readonly ulong expectedGuildId = 1234567890123456789;
        private readonly ulong expectedUserId = 9876543210987654321;
        private readonly ulong expectedRoleId = 1111111111111111111;

        public UserManagementServiceTest()
        {
            var mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockGuildFactory = new Mock<IGuildFactory>();
            _mockClientService = new Mock<ClientService>(mockDiscordClient.Object, _mockGuildFactory.Object);
            _mockGuildService = new Mock<GuildService>(_mockClientService.Object);
            _userManagementService = new UserManagementService(_mockGuildService.Object);
        }

        [Test]
        public async Task AssignTroubleRoleToUserById_WithValidParameters_AssignsRole()
        {
            _mockGuildService.Setup(g => g.GetRoleIdByName(It.IsAny<ulong>(), "trouble")).Returns(expectedRoleId);
            await _userManagementService.AssignTroubleRoleToUserById(expectedGuildId, expectedUserId);
            _mockGuildService.Verify(g => g.AddRoleToUserByIdAsync(expectedGuildId, expectedUserId, expectedRoleId), Times.Once);
        }

        [Test]
        public async Task AssignTroubleRoleToUserById_WithInvalidRoleName_DoesNotAssignRole()
        {
            _mockGuildService.Setup(g => g.GetRoleIdByName(It.IsAny<ulong>(), "bingus")).Returns(0);
            await _userManagementService.AssignTroubleRoleToUserById(expectedGuildId, expectedUserId);
            _mockGuildService.Verify(g => g.AddRoleToUserByIdAsync(It.IsAny<ulong>(), It.IsAny<ulong>(), It.IsAny<ulong>()), Times.Never);
        }
    }
}

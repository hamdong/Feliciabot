using Discord.WebSocket;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests
{
    [TestFixture]
    public class UserManagementServiceTest
    {
        private readonly UserManagementService _userManagementService;
        private readonly Mock<GuildService> _mockGuildService;

        private readonly ulong expectedGuildId = GenerateRandomUlong();
        private readonly ulong expectedUserId = GenerateRandomUlong();
        private readonly ulong expectedRoleId = GenerateRandomUlong();

        public UserManagementServiceTest()
        {
            var mockDiscordClient = new Mock<DiscordSocketClient>();
            _mockGuildService = new Mock<GuildService>(mockDiscordClient.Object);
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

        private static ulong GenerateRandomUlong()
        {
            var random = new Random();
            byte[] randomNumberBytes = new byte[8];
            random.NextBytes(randomNumberBytes);
            return BitConverter.ToUInt64(randomNumberBytes, 0);
        }
    }
}

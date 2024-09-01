using Discord;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class UserManagementServiceTest
    {
        private readonly Mock<IRole> mockRole;
        private readonly Mock<IGuild> mockGuild;
        private readonly Mock<IGuildUser> mockUser;
        private readonly UserManagementService userManagementService;

        private readonly ulong expectedRoleId = 1111111111111111111;

        public UserManagementServiceTest()
        {
            mockRole = new Mock<IRole>();
            mockGuild = new Mock<IGuild>();
            mockUser = new Mock<IGuildUser>();
            userManagementService = new UserManagementService();
        }

        [SetUp]
        public void Setup()
        {
            mockRole.SetupGet(r => r.Id).Returns(expectedRoleId);
            mockRole.SetupGet(r => r.Name).Returns("trouble");
            List<IRole> roles = [mockRole.Object];
            mockGuild.SetupGet(g => g.Roles).Returns(roles.AsReadOnly());
            mockUser.SetupGet(u => u.RoleIds).Returns([]);
            mockUser.SetupGet(u => u.Guild).Returns(mockGuild.Object);
        }

        [Test]
        public async Task AssignTroubleRoleToUserById_WithNoRoleFound_Exits()
        {
            mockGuild.SetupGet(g => g.Roles).Returns([]);

            await userManagementService.AssignTroubleRoleToUserById(mockUser.Object);

            mockUser.Verify(u => u.AddRoleAsync(It.IsAny<ulong>(), null), Times.Never);
        }

        [Test]
        public async Task AssignTroubleRoleToUserById_WithUserHasRole_Exits()
        {
            mockUser.SetupGet(g => g.RoleIds).Returns([expectedRoleId]);

            await userManagementService.AssignTroubleRoleToUserById(mockUser.Object);

            mockUser.Verify(u => u.AddRoleAsync(It.IsAny<ulong>(), null), Times.Never);
        }

        [Test]
        public async Task AssignTroubleRoleToUserById_WithValidParameters_AssignsRole()
        {
            await userManagementService.AssignTroubleRoleToUserById(mockUser.Object);

            mockUser.Verify(u => u.AddRoleAsync(It.IsAny<ulong>(), null), Times.Once);
        }
    }
}

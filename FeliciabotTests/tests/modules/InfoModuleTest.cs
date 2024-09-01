using Discord;
using Feliciabot.net._6._0.modules;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class InfoModuleTest
    {
        private readonly Mock<IGuild> mockGuild;
        private readonly Mock<IActivity> mockActivity;
        private readonly Mock<ISelfUser> mockSelfUser;
        private readonly Mock<IDiscordClient> mockClient;
        private readonly Mock<IDiscordInteraction> mockDiscordInteraction;
        private readonly Mock<IInteractionContext> mockContext;
        private readonly InfoModule infoModule;

        public InfoModuleTest()
        {
            mockGuild = new Mock<IGuild>();
            mockActivity = new Mock<IActivity>();
            mockSelfUser = new Mock<ISelfUser>();
            mockClient = new Mock<IDiscordClient>();
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            infoModule = new InfoModule();
        }

        [SetUp]
        public void Setup()
        {
            mockActivity.SetupGet(a => a.Name).Returns("Activity");
            List<IActivity> activities = [mockActivity.Object];
            mockSelfUser.SetupGet(u => u.Username).Returns("Username");
            mockSelfUser.SetupGet(u => u.Status).Returns(UserStatus.Online);
            mockSelfUser.SetupGet(u => u.Activities).Returns(activities.AsReadOnly());
            List<IGuild> guilds = [mockGuild.Object];
            mockClient.Setup(c => c.GetGuildsAsync(CacheMode.AllowDownload, null)).ReturnsAsync(guilds.AsReadOnly());
            mockClient.SetupGet(c => c.CurrentUser).Returns(mockSelfUser.Object);
            mockContext.SetupGet(c => c.Client).Returns(mockClient.Object);
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            MockContextHelper.SetContext(infoModule, mockContext.Object);
        }

        [Test]
        public async Task Info_DmsUser()
        {
            await infoModule.Info();

            mockContext.Verify(
                c =>
                    c.Interaction.RespondAsync(
                        null,
                        null,
                        false,
                        false,
                        null,
                        null,
                        It.IsAny<Embed>(),
                        null,
                        null
                    ),
                Times.Once
            );
        }
    }
}

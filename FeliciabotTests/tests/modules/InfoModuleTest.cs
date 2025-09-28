using Discord;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
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
        private readonly Mock<IEmbedBuilderService> mockEmbedBuilderService;
        private readonly InfoModule infoModule;

        public InfoModuleTest()
        {
            mockGuild = new Mock<IGuild>();
            mockActivity = new Mock<IActivity>();
            mockSelfUser = new Mock<ISelfUser>();
            mockClient = new Mock<IDiscordClient>();
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            mockEmbedBuilderService = new Mock<IEmbedBuilderService>();
            infoModule = new InfoModule(mockEmbedBuilderService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockActivity.SetupGet(a => a.Name).Returns("Activity");
            List<IActivity> activities = [mockActivity.Object];
            mockSelfUser.SetupGet(u => u.Username).Returns("Username");
            mockSelfUser.SetupGet(u => u.Status).Returns(UserStatus.Online);
            mockSelfUser.SetupGet(u => u.Activities).Returns(activities.AsReadOnly());
            mockSelfUser
                .Setup(u => u.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://discord.com");
            List<IGuild> guilds = [mockGuild.Object];
            mockClient
                .Setup(c => c.GetGuildsAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(guilds.AsReadOnly());
            mockClient.SetupGet(c => c.CurrentUser).Returns(mockSelfUser.Object);
            mockContext.SetupGet(c => c.Client).Returns(mockClient.Object);
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            MockContextHelper.SetContext(infoModule, mockContext.Object);
        }

        [Test]
        public async Task Info_RespondsWithEmbed()
        {
            var expectedBotInfo =
                $"Name: Username\n"
                + $"Created by: Ham#1185\n"
                + $"Framework: Discord.NET C#\n"
                + $"Status: Online\n"
                + $"Currently playing: Activity\n"
                + $"Currently in: 1 servers!";
            var builder = new EmbedBuilder();
            builder.WithTitle("You want to know more about me?");
            builder.AddField("Bot Info", expectedBotInfo);
            builder.WithThumbnailUrl("https://discord.com");
            builder.WithColor(Color.LightGrey);
            var expectedEmbed = builder.Build();
            mockEmbedBuilderService
                .Setup(s => s.GetBotInfoAsEmbed(It.IsAny<string>()))
                .Returns(expectedEmbed);

            await infoModule.Info();

            mockContext.Verify(
                c =>
                    c.Interaction.RespondAsync(
                        It.IsAny<string>(),
                        It.IsAny<Embed[]>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>(),
                        It.IsAny<AllowedMentions>(),
                        It.IsAny<MessageComponent>(),
                        expectedEmbed,
                        It.IsAny<RequestOptions>(),
                        It.IsAny<PollProperties>(),
                        It.IsAny<MessageFlags>()
                    ),
                Times.Once
            );
        }
    }
}

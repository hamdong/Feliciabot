using Discord;
using Discord.Interactions;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive.Pagination;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class HelpModuleTest
    {
        private readonly Mock<IUser> mockUser;
        private readonly Mock<IDiscordInteraction> mockDiscordInteraction;
        private readonly Mock<IInteractionContext> mockContext;
        private readonly Mock<IInteractiveHelperService> mockInteractiveHelperService;
        private readonly HelpModule helpModule;

        public HelpModuleTest()
        {
            mockUser = new Mock<IUser>();
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            mockInteractiveHelperService = new Mock<IInteractiveHelperService>();
            helpModule = new HelpModule(mockInteractiveHelperService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockDiscordInteraction.Reset();
            mockInteractiveHelperService.Reset();
            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns([("Name1", "Description1"), ("Name2", "Description2")]);
            mockContext.SetupGet(c => c.User).Returns(mockUser.Object);
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            MockContextHelper.SetContext(helpModule, mockContext.Object);
        }

        [Test]
        public async Task HelpInfo_Responds()
        {
            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns([("Info", "Description")]);
            await helpModule.HelpInfo();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRolePlay_Responds()
        {
            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns([("RolePlay", "Description")]);
            await helpModule.HelpRolePlay();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRoll_Responds()
        {
            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns([("Roll", "Description")]);
            await helpModule.HelpRoll();
            VerifyPaginator();
        }

        [Test]
        public async Task Help_WhenMultipleCommands_RespondsWithPages()
        {
            List<(string Name, string Description)> exampleCommands = new();
            for (int i = 0; i < 13; i++)
            {
                exampleCommands.Add(("Name", "Description"));
            }
            var commands = exampleCommands.ToArray();

            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns(commands);
            await helpModule.HelpRoll();
            VerifyPaginator();
        }

        [Test]
        public async Task Help_WhenNoCommands_RespondsWithError()
        {
            mockInteractiveHelperService
                .Setup(s => s.GetNonHelpSlashCommandsByName(It.IsAny<string>()))
                .Returns([]);
            await helpModule.HelpRoll();
            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Contains("No modules found for 'Roll'")
            );
        }

        private void VerifyPaginator()
        {
            mockInteractiveHelperService.Verify(
                s =>
                    s.SendPaginatorAsync(
                        It.IsAny<IInteractionContext>(),
                        It.IsAny<StaticPaginator>()
                    ),
                Times.Once
            );
            VerifyHelper.VerifyInteractionAsync(mockContext, s => s.Contains("Commands"));
        }
    }
}

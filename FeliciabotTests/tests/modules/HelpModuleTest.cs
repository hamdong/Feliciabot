using Discord;
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
        private readonly Mock<IDiscordInteraction> mockDiscordInteraction;
        private readonly Mock<IInteractionContext> mockContext;
        private readonly Mock<IPaginatorService> mockPaginatorService;
        private readonly HelpModule helpModule;

        public HelpModuleTest()
        {
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            mockPaginatorService = new Mock<IPaginatorService>();
            helpModule = new HelpModule(mockPaginatorService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockDiscordInteraction.Reset();
            mockPaginatorService.Reset();
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            MockContextHelper.SetContext(helpModule, mockContext.Object);
        }

        [Test]
        public async Task HelpInfo_Responds()
        {
            await helpModule.HelpInfo();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpMusic_Responds()
        {
            await helpModule.HelpMusic();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRolePlay_Responds()
        {
            await helpModule.HelpRolePlay();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRoll_Responds()
        {
            await helpModule.HelpRoll();
            VerifyPaginator();
        }

        private void VerifyPaginator()
        {
            mockPaginatorService.Verify(
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

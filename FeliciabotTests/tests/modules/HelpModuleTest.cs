using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<IPaginatorService> _mockPaginatorService;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly HelpModule _helpModule;

        public HelpModuleTest()
        {
            _mockContext = new Mock<IInteractionContext>();
            _mockPaginatorService = new Mock<IPaginatorService>();
            _mockInteractingService = new Mock<IInteractingService>();
            _helpModule = new HelpModule(
                _mockPaginatorService.Object,
                _mockInteractingService.Object
            );
        }

        [SetUp]
        public void Setup()
        {
            _mockPaginatorService.Reset();
            MockContextHelper.SetContext(_helpModule, _mockContext.Object);
        }

        [Test]
        public async Task HelpInfo_Responds()
        {
            await _helpModule.HelpInfo();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpMusic_Responds()
        {
            await _helpModule.HelpMusic();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRolePlay_Responds()
        {
            await _helpModule.HelpRolePlay();
            VerifyPaginator();
        }

        [Test]
        public async Task HelpRoll_Responds()
        {
            await _helpModule.HelpRoll();
            VerifyPaginator();
        }

        private void VerifyPaginator()
        {
            _mockPaginatorService.Verify(
                s =>
                    s.SendPaginatorAsync(
                        It.IsAny<SocketInteractionContext<SocketInteraction>>(),
                        It.IsAny<StaticPaginator>()
                    ),
                Times.Once
            );
        }
    }
}

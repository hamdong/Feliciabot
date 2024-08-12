using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class InfoModuleTest
    {
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly InfoModule _infoModule;

        public InfoModuleTest()
        {
            _mockContext = new Mock<IInteractionContext>();
            _mockClientService = new Mock<IClientService>();
            _mockInteractingService = new Mock<IInteractingService>();
            _infoModule = new InfoModule(_mockClientService.Object, _mockInteractingService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockInteractingService.Reset();
            TestCommandContext.SetContext(_infoModule, _mockContext.Object);
        }

        [Test]
        public async Task Info_DmsUser()
        {
            _mockClientService.Setup(c => c.GetUsername()).Returns("username");
            _mockClientService.Setup(c => c.GetStatus()).Returns(UserStatus.Online);
            _mockClientService.Setup(c => c.GetActivities()).Returns([]);

            await _infoModule.Info();

            _mockInteractingService.Verify(s => s.SendResponseToUserAsync(It.IsAny<SocketInteractionContext<SocketInteraction>>(), It.IsAny<Embed>()), Times.Once);
        }
    }
}

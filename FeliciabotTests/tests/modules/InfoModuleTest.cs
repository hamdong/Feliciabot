using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class InfoModuleTest
    {
        private readonly Mock<IActivity> _mockActivity;
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly InfoModule _infoModule;

        public InfoModuleTest()
        {
            _mockActivity = new Mock<IActivity>();
            _mockContext = new Mock<IInteractionContext>();
            _mockClientService = new Mock<IClientService>();
            _mockInteractingService = new Mock<IInteractingService>();
            _infoModule = new InfoModule(_mockClientService.Object, _mockInteractingService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockActivity.Setup(a => a.Name).Returns("Activity");
            _mockInteractingService.Reset();
            MockContextHelper.SetContext(_infoModule, _mockContext.Object);
        }

        [Test]
        public async Task Info_DmsUser()
        {
            _mockClientService.Setup(c => c.GetClient()).Returns(new Client("username", UserStatus.Online, _mockActivity.Object));

            await _infoModule.Info();

            _mockInteractingService.Verify(s => s.SendResponseToUserAsync(It.IsAny<SocketInteractionContext<SocketInteraction>>(), It.IsAny<Embed>()), Times.Once);
        }
    }
}

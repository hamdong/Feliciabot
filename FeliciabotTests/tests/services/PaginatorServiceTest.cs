using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.Abstractions.models;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Fergun.Interactive;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class PaginatorServiceTest
    {
        private readonly Mock<SocketInteractionContext> _mockContext;
        private readonly Mock<DiscordSocketClient> _mockClient;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly Mock<InteractiveService> _mockInteractiveService;
        private readonly PaginatorService _paginatorService;

        public PaginatorServiceTest()
        {
            _mockContext = new Mock<SocketInteractionContext>();
            _mockClient = new Mock<DiscordSocketClient>();
            _mockInteractingService = new Mock<IInteractingService>();
            _mockInteractiveService = new Mock<InteractiveService>(_mockClient.Object);
            _paginatorService = new PaginatorService(_mockInteractingService.Object, _mockInteractiveService.Object);
        }

        [Test]
        public void BuildModulesPaginator_BuildsPaginator()
        {
            List<SlashCommand> expectedSlashCommands = [new SlashCommand("Test", "Description", "Module")];
            _mockInteractingService.Setup(s => s.GetSlashCommands()).Returns(expectedSlashCommands.AsReadOnly());

            var result = _paginatorService.BuildModulesPaginator(_mockContext.Object, "Test");

            Assert.That(result.Pages.Count, Is.EqualTo(2));
        }
    }
}

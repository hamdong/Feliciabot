using Discord;
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
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<DiscordSocketClient> _mockClient;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly Mock<InteractiveService> _mockInteractiveService;
        private readonly PaginatorService _paginatorService;

        public PaginatorServiceTest()
        {
            _mockContext = new Mock<IInteractionContext>();
            _mockClient = new Mock<DiscordSocketClient>();
            _mockInteractingService = new Mock<IInteractingService>();
            _mockInteractiveService = new Mock<InteractiveService>(_mockClient.Object);
            _paginatorService = new PaginatorService(
                _mockInteractingService.Object,
                _mockInteractiveService.Object
            );
        }

        [SetUp]
        public void Setup()
        {
            Mock<IUser> testUser = new();
            Mock<IMessageChannel> testChannel = new();
            _mockContext.SetupGet(c => c.User).Returns(testUser.Object);
            _mockContext.SetupGet(c => c.Channel).Returns(testChannel.Object);
            MockContextHelper.SetContext(_paginatorService, _mockContext.Object);
        }

        [Test]
        public void BuildModulesPaginator_BuildsPaginator()
        {
            List<SlashCommand> expectedSlashCommands = [];
            for (int i = 0; i < 13; i++)
            {
                expectedSlashCommands.Add(new SlashCommand("Test", "Description", "ModuleName"));
            }
            _mockInteractingService
                .Setup(s => s.GetSlashCommands())
                .Returns(expectedSlashCommands.AsReadOnly());

            var result = _paginatorService.BuildModulesPaginator(_mockContext.Object, "ModuleName");

            Assert.That(result.Pages.Count, Is.EqualTo(2));
        }
    }
}

using Discord;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class GreetingServiceTest
    {
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IUserManagementService> _mockUserManagementService;
        private readonly Mock<IRandomizerService> _mockRandomizerService;
        private readonly GreetingService greetingService;
        private readonly TestDiscordEnv testDiscordEnv;

        public GreetingServiceTest()
        {
            testDiscordEnv = new TestDiscordEnv();
            _mockClientService = new Mock<IClientService>();
            _mockUserManagementService = new Mock<IUserManagementService>();
            _mockRandomizerService = new Mock<IRandomizerService>();
            greetingService = new GreetingService(
                _mockClientService.Object,
                _mockUserManagementService.Object,
                _mockRandomizerService.Object
            );
        }

        [SetUp]
        public void Setup()
        {
            testDiscordEnv.mockUserMessage.Reset();
            _mockClientService.Setup(s => s.GetClientId()).Returns(testDiscordEnv.selfUserId);
        }

        [Test]
        public async Task ReplyToNonCommand_WithNoMention_ShouldNotRespond()
        {
            testDiscordEnv.mockUserMessage.SetupGet(m => m.MentionedUserIds).Returns([]);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            _mockRandomizerService.Verify(s => s.GetRandom(It.IsAny<int>(), 0), Times.Never());
        }

        [Test]
        public async Task ReplyToNonCommand_WithMatchingAuthor_ShouldNotRespond()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.MentionedUserIds)
                .Returns([testDiscordEnv.selfUserId]);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Author)
                .Returns(testDiscordEnv.mockSelfUserAsIUser.Object);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            _mockRandomizerService.Verify(s => s.GetRandom(It.IsAny<int>(), 0), Times.Never());
        }

        [Test]
        public async Task ReplyToNonCommand_WithReference_ShouldNotRespond()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.MentionedUserIds)
                .Returns([testDiscordEnv.selfUserId]);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Author)
                .Returns(testDiscordEnv.mockUser.Object);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Reference)
                .Returns(testDiscordEnv.mockMessageReference.Object);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            _mockRandomizerService.Verify(s => s.GetRandom(It.IsAny<int>(), 0), Times.Never());
        }

        [Test]
        public async Task ReplyToNonCommand_WithMentionDiffAuthorNoRef_ShouldRespond()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.MentionedUserIds)
                .Returns([testDiscordEnv.selfUserId]);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Author)
                .Returns(testDiscordEnv.mockUser.Object);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Reference)
                .Returns((MessageReference)null!);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            _mockRandomizerService.Verify(s => s.GetRandom(It.IsAny<int>(), 0), Times.Never());
        }
    }
}

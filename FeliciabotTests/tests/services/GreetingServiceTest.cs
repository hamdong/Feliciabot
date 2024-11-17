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
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.MentionedUserIds)
                .Returns([testDiscordEnv.selfUserId]);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Author)
                .Returns(testDiscordEnv.mockUser.Object);
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Reference)
                .Returns((MessageReference)null!);
            testDiscordEnv
                .mockGuildUser.SetupGet(m => m.Guild)
                .Returns(testDiscordEnv.mockGuild.Object);
            _mockClientService.Setup(s => s.GetClientId()).Returns(testDiscordEnv.selfUserId);
        }

        [Test]
        public async Task ReplyToNonCommand_WithNoMention_ShouldExit()
        {
            testDiscordEnv.mockUserMessage.SetupGet(m => m.MentionedUserIds).Returns([]);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockMessageChannel);
        }

        [Test]
        public async Task ReplyToNonCommand_WithMatchingAuthor_ShouldExit()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Author)
                .Returns(testDiscordEnv.mockSelfUserAsIUser.Object);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockMessageChannel);
        }

        [Test]
        public async Task ReplyToNonCommand_WithReference_ShouldExit()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Reference)
                .Returns(testDiscordEnv.mockMessageReference.Object);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockMessageChannel);
        }

        [Test]
        public async Task ReplyToNonCommand_WithNonTextChannel_ShouldExit()
        {
            testDiscordEnv.mockUserMessage.SetupGet(m => m.Channel).Returns((IMessageChannel)null!);

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockMessageChannel);
        }

        [Test]
        public async Task ReplyToNonCommand_WithTextChannelAndReaction_ShouldRespondWithReact()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Channel)
                .Returns(testDiscordEnv.mockMessageChannel.Object);
            testDiscordEnv.mockUserMessage.SetupGet(m => m.Content).Returns("hi");

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyMessageSentAsync(
                testDiscordEnv.mockMessageChannel,
                s => s.Equals("Hi N-Nice to see you!")
            );
        }

        [Test]
        public async Task ReplyToNonCommand_WithTextChannelAndNoReact_ShouldRespondWithQuote()
        {
            testDiscordEnv
                .mockUserMessage.SetupGet(m => m.Channel)
                .Returns(testDiscordEnv.mockMessageChannel.Object);
            testDiscordEnv.mockUserMessage.SetupGet(m => m.Content).Returns("");

            await greetingService.ReplyToNonCommand(testDiscordEnv.mockUserMessage.Object);

            VerifyHelper.VerifyMessageSentAsync(
                testDiscordEnv.mockMessageChannel,
                s => s.Length != 0
            );
        }

        [Test]
        public async Task HandleOnUserJoined_WithNoGuild_ShouldExit()
        {
            testDiscordEnv.mockGuildUser.SetupGet(m => m.Guild).Returns((IGuild)null!);

            await greetingService.HandleOnUserJoined(testDiscordEnv.mockGuildUser.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockSystemChannel);
        }

        [Test]
        public async Task HandleOnUserJoined_WithNoSystemChannel_ShouldExit()
        {
            testDiscordEnv.mockGuild.SetupGet(g => g.SystemChannelId).Returns(0);

            await greetingService.HandleOnUserJoined(testDiscordEnv.mockGuildUser.Object);

            VerifyHelper.VerifyNoMessageSentAsync(testDiscordEnv.mockSystemChannel);
        }

        [Test]
        public async Task HandleOnUserJoined_WithSystemChannel_ShouldWelcomeUser()
        {
            testDiscordEnv
                .mockGuild.SetupGet(g => g.SystemChannelId)
                .Returns(testDiscordEnv.systemChannelId);
            testDiscordEnv
                .mockGuild.Setup(g =>
                    g.GetTextChannelAsync(
                        testDiscordEnv.systemChannelId,
                        CacheMode.AllowDownload,
                        null
                    )
                )
                .ReturnsAsync(testDiscordEnv.mockSystemChannel.Object);

            await greetingService.HandleOnUserJoined(testDiscordEnv.mockGuildUser.Object);

            VerifyHelper.VerifyMessageSentAsync(
                testDiscordEnv.mockSystemChannel,
                s => s.Contains("Welcome to")
            );
        }
    }
}

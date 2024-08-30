using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands.fun;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class TeaCommandTest
    {
        private readonly Mock<IDiscordClient> _mockClient;
        private readonly Mock<ISelfUser> _mockSelfUser;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IRandomizerService> _mockRandomizerService;
        private readonly TeaCommand _teaCommand;

        public TeaCommandTest()
        {
            _mockClient = new Mock<IDiscordClient>();
            _mockSelfUser = new Mock<ISelfUser>();
            _mockUser = new Mock<IUser>();
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _mockRandomizerService = new Mock<IRandomizerService>();
            _teaCommand = new TeaCommand(_mockRandomizerService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockChannel.Reset();
            _mockSelfUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            _mockClient.SetupGet(c => c.CurrentUser).Returns(_mockSelfUser.Object);
            _mockUser.SetupGet(u => u.GlobalName).Returns("GlobalName");
            _mockContext.SetupGet(c => c.Client).Returns(_mockClient.Object);
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
            _mockContext.SetupGet(c => c.User).Returns(_mockUser.Object);
            MockContextHelper.SetContext(_teaCommand, _mockContext.Object);
        }

        [Test]
        public async Task Tea_WhenNoUserAndHighRolls_ServesToCaller()
        {
            _mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            await _teaCommand.Tea();

            MockContextHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Contains("Served") && s.Contains(_mockUser.Object.GlobalName)
            );
        }

        [Test]
        public async Task Tea_WhenNoUserAndLowRolls_SpillsOnCaller()
        {
            _mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            await _teaCommand.Tea();

            MockContextHelper.VerifyMessageSentAsync(
                _mockChannel,
                s =>
                    s.Contains("Spilled")
                    && s.Contains(_mockUser.Object.GlobalName)
                    && s.Contains($"Sorry {_mockUser.Object.GlobalName}!")
            );
        }

        [Test]
        public async Task Tea_WhenCurrentUserAndHighRolls_ServesToUser()
        {
            _mockUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            _mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            await _teaCommand.Tea(_mockUser.Object);

            MockContextHelper.VerifyMessageSentAsync(
                _mockChannel,
                s =>
                    s.Contains("Served")
                    && s.Contains(_mockUser.Object.GlobalName)
                    && s.Contains("Thanks! :smile:")
            );
        }

        [Test]
        public async Task Tea_WhenCurrentUserAndLowRolls_SpillsOnUser()
        {
            _mockUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            _mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            await _teaCommand.Tea(_mockUser.Object);

            MockContextHelper.VerifyMessageSentAsync(
                _mockChannel,
                s =>
                    s.Contains("Spilled")
                    && s.Contains(_mockUser.Object.GlobalName)
                    && s.Contains("Oh no!")
            );
        }

        [Test]
        public async Task Tea_WhenCurrentUserAndRollsMilk_Exclaims()
        {
            _mockUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            _mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(18);

            await _teaCommand.Tea(_mockUser.Object);

            MockContextHelper.VerifyMessageSentAsync(
                _mockChannel,
                s =>
                    s.Contains("Served")
                    && s.Contains(_mockUser.Object.GlobalName)
                    && s.Contains("Oh no! I'm allergic to milk! :scream:")
            );
        }
    }
}

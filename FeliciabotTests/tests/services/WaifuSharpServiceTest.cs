using Discord;
using Feliciabot.net._6._0.services;
using Moq;
using NUnit.Framework;
using WaifuSharp;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class WaifuSharpServiceTest
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<WaifuClient> _mockWaifuClient;
        private readonly WaifuSharpService _waifuSharpService;

        public WaifuSharpServiceTest()
        {
            _mockUser = new Mock<IUser>();
            _mockContext = new Mock<IInteractionContext>();
            _mockWaifuClient = new Mock<WaifuClient>();
            _waifuSharpService = new WaifuSharpService(_mockWaifuClient.Object);
        }

        [SetUp]
        public void Setup()
        {
            Mock<IDiscordInteraction> mockInteraction = new();
            _mockUser.SetupGet(u => u.GlobalName).Returns("Caller");
            _mockUser.SetupGet(u => u.Mention).Returns("Receiver");
            _mockContext.SetupGet(c => c.User).Returns(_mockUser.Object);
            _mockContext.SetupGet(c => c.Interaction).Returns(mockInteraction.Object);
            TestCommandContext.SetContext(_waifuSharpService, _mockContext.Object);
        }

        [Test]
        public async Task SendWaifuSharpResponseAsync_PostsResponse()
        {
            await _waifuSharpService.SendWaifuSharpResponseAsync(
                _mockContext.Object,
                Endpoints.Sfw.Bite,
                $"bit {_mockUser.Object.Mention}"
            );

            _mockContext.Verify(
                c =>
                    c.Interaction.RespondAsync(
                        It.IsAny<string>(),
                        null,
                        false,
                        false,
                        null,
                        null,
                        It.IsAny<Embed>(),
                        null,
                        null
                    ),
                Times.Once
            );
        }
    }
}

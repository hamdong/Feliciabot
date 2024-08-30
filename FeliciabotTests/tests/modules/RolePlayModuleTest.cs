using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;
using WaifuSharp;

namespace FeliciabotTests.tests.modules
{
    public class RolePlayModuleTest
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<IWaifuSharpService> _mockWaifuSharpService;
        private readonly RolePlayModule _rolePlayModule;

        public RolePlayModuleTest()
        {
            _mockUser = new Mock<IUser>();
            _mockContext = new Mock<IInteractionContext>();
            _mockWaifuSharpService = new Mock<IWaifuSharpService>();
            _rolePlayModule = new RolePlayModule(_mockWaifuSharpService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockWaifuSharpService
                .Setup(s =>
                    s.SendWaifuSharpResponseAsync(
                        It.IsAny<SocketInteractionContext<SocketInteraction>>(),
                        It.IsAny<Endpoints.Sfw>(),
                        It.IsAny<string>()
                    )
                )
                .Returns(Task.CompletedTask);
            _mockUser.SetupGet(u => u.GlobalName).Returns("TestGlobalName");
            _mockUser.SetupGet(u => u.Mention).Returns("TestGlobalName");
            _mockContext.SetupGet(c => c.User).Returns(_mockUser.Object);
            MockContextHelper.SetContext(_rolePlayModule, _mockContext.Object);
        }

        [Test]
        public async Task Bite_Responds()
        {
            await _rolePlayModule.Bite(_mockUser.Object);
            VerifyResponse("bit", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Blush_Responds()
        {
            await _rolePlayModule.Blush();
            VerifyResponse("blushing");
        }

        [Test]
        public async Task Bully_Responds()
        {
            await _rolePlayModule.Bully(_mockUser.Object);
            VerifyResponse("bullied", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Cringe_Responds()
        {
            await _rolePlayModule.Cringe();
            VerifyResponse("cringing");
        }

        [Test]
        public async Task Cry_Responds()
        {
            await _rolePlayModule.Cry();
            VerifyResponse("crying");
        }

        [Test]
        public async Task Dance_Responds()
        {
            await _rolePlayModule.Dance();
            VerifyResponse("dancing");
        }

        [Test]
        public async Task Happy_Responds()
        {
            await _rolePlayModule.Happy();
            VerifyResponse("happy");
        }

        [Test]
        public async Task HighFive_Responds()
        {
            await _rolePlayModule.Highfive(_mockUser.Object);
            VerifyResponse("high-fived", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Hug_Responds()
        {
            await _rolePlayModule.Hug(_mockUser.Object);
            VerifyResponse("hugged", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Kiss_Responds()
        {
            await _rolePlayModule.Kiss(_mockUser.Object);
            VerifyResponse("kissed", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Nom_Responds()
        {
            await _rolePlayModule.Nom(_mockUser.Object);
            VerifyResponse("nommed", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Lick_Responds()
        {
            await _rolePlayModule.Lick(_mockUser.Object);
            VerifyResponse("licked", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Pat_Responds()
        {
            await _rolePlayModule.Pat(_mockUser.Object);
            VerifyResponse("patted", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Poke_Responds()
        {
            await _rolePlayModule.Poke(_mockUser.Object);
            VerifyResponse("poked", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Slap_Responds()
        {
            await _rolePlayModule.Slap(_mockUser.Object);
            VerifyResponse("slapped", _mockUser.Object.Mention);
        }

        [Test]
        public async Task Smug_Responds()
        {
            await _rolePlayModule.Smug();
            VerifyResponse("smug");
        }

        [Test]
        public async Task Wink_Responds()
        {
            await _rolePlayModule.Wink();
            VerifyResponse("winking");
        }

        private void VerifyResponse(string containsQuery, string userMention = "")
        {
            _mockWaifuSharpService.Verify(
                c =>
                    c.SendWaifuSharpResponseAsync(
                        It.IsAny<SocketInteractionContext<SocketInteraction>>(),
                        It.IsAny<Endpoints.Sfw>(),
                        It.Is<string>(s => s.Contains(containsQuery) && s.Contains(userMention))
                    ),
                Times.Once
            );
        }
    }
}

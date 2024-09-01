using System.Linq;
using Discord;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;
using WaifuSharp;

namespace FeliciabotTests.tests.modules
{
    public class RolePlayModuleTest
    {
        private readonly Mock<IUser> mockUser;
        private readonly Mock<IDiscordInteraction> mockDiscordInteraction;
        private readonly Mock<IInteractionContext> mockContext;
        private readonly Mock<IWaifuSharpService> mockWaifuService;
        private readonly RolePlayModule rolePlayModule;

        public RolePlayModuleTest()
        {
            mockUser = new Mock<IUser>();
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            mockWaifuService = new Mock<IWaifuSharpService>();
            rolePlayModule = new RolePlayModule(mockWaifuService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockDiscordInteraction.Reset();
            mockWaifuService.Setup(s => s.GetSfwImage(It.IsAny<Endpoints.Sfw>())).Returns("https://picsum.photos/200");
            mockUser.SetupGet(u => u.GlobalName).Returns("TestGlobalName");
            mockUser.SetupGet(u => u.Mention).Returns("TestGlobalName");
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            mockContext.SetupGet(c => c.User).Returns(mockUser.Object);
            MockContextHelper.SetContext(rolePlayModule, mockContext.Object);
        }

        [Test]
        public async Task Bite_Responds()
        {
            await rolePlayModule.Bite(mockUser.Object);
            VerifyResponse("bit");
        }

        [Test]
        public async Task Blush_Responds()
        {
            await rolePlayModule.Blush();
            VerifyResponse("blushing");
        }

        [Test]
        public async Task Bully_Responds()
        {
            await rolePlayModule.Bully(mockUser.Object);
            VerifyResponse("bullied", mockUser.Object.Mention);
        }

        [Test]
        public async Task Cringe_Responds()
        {
            await rolePlayModule.Cringe();
            VerifyResponse("cringing");
        }

        [Test]
        public async Task Cry_Responds()
        {
            await rolePlayModule.Cry();
            VerifyResponse("crying");
        }

        [Test]
        public async Task Dance_Responds()
        {
            await rolePlayModule.Dance();
            VerifyResponse("dancing");
        }

        [Test]
        public async Task Happy_Responds()
        {
            await rolePlayModule.Happy();
            VerifyResponse("happy");
        }

        [Test]
        public async Task HighFive_Responds()
        {
            await rolePlayModule.Highfive(mockUser.Object);
            VerifyResponse("high-fived", mockUser.Object.Mention);
        }

        [Test]
        public async Task Hug_Responds()
        {
            await rolePlayModule.Hug(mockUser.Object);
            VerifyResponse("hugged", mockUser.Object.Mention);
        }

        [Test]
        public async Task Kiss_Responds()
        {
            await rolePlayModule.Kiss(mockUser.Object);
            VerifyResponse("kissed", mockUser.Object.Mention);
        }

        [Test]
        public async Task Nom_Responds()
        {
            await rolePlayModule.Nom(mockUser.Object);
            VerifyResponse("nommed", mockUser.Object.Mention);
        }

        [Test]
        public async Task Lick_Responds()
        {
            await rolePlayModule.Lick(mockUser.Object);
            VerifyResponse("licked", mockUser.Object.Mention);
        }

        [Test]
        public async Task Pat_Responds()
        {
            await rolePlayModule.Pat(mockUser.Object);
            VerifyResponse("patted", mockUser.Object.Mention);
        }

        [Test]
        public async Task Poke_Responds()
        {
            await rolePlayModule.Poke(mockUser.Object);
            VerifyResponse("poked", mockUser.Object.Mention);
        }

        [Test]
        public async Task Slap_Responds()
        {
            await rolePlayModule.Slap(mockUser.Object);
            VerifyResponse("slapped", mockUser.Object.Mention);
        }

        [Test]
        public async Task Smug_Responds()
        {
            await rolePlayModule.Smug();
            VerifyResponse("smug");
        }

        [Test]
        public async Task Wink_Responds()
        {
            await rolePlayModule.Wink();
            VerifyResponse("winking");
        }

        private void VerifyResponse(string containsQuery, string userMention = "")
        {
            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Contains(containsQuery) && s.Contains(userMention)
            );
        }
    }
}

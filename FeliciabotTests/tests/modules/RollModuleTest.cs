using Discord;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;
using WaifuSharp;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class RollModuleTest
    {
        private readonly Mock<IUser> mockUser;
        private readonly Mock<IUser> mockBotUser;
        private readonly Mock<IDiscordInteraction> mockDiscordInteraction;
        private readonly Mock<IInteractionContext> mockContext;
        private readonly Mock<IWaifuSharpService> mockWaifuService;
        private readonly RollModule rollModule;

        public RollModuleTest()
        {
            mockUser = new Mock<IUser>();
            mockBotUser = new Mock<IUser>();
            mockDiscordInteraction = new Mock<IDiscordInteraction>();
            mockContext = new Mock<IInteractionContext>();
            mockWaifuService = new Mock<IWaifuSharpService>();
            rollModule = new RollModule(mockWaifuService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockDiscordInteraction.Reset();
            mockWaifuService.Setup(s => s.GetSfwImage(It.IsAny<Endpoints.Sfw>())).Returns("https://picsum.photos/200");
            mockUser.SetupGet(u => u.GlobalName).Returns("GlobalName");
            mockBotUser.SetupGet(u => u.IsBot).Returns(true);
            mockContext.SetupGet(c => c.User).Returns(mockUser.Object);
            mockContext.SetupGet(c => c.Interaction).Returns(mockDiscordInteraction.Object);
            MockContextHelper.SetContext(rollModule, mockContext.Object);
        }

        [Test]
        public async Task EightBall_RollsResponse()
        {
            var flatResponses = Responses.RollResponses.SelectMany(response => response);

            await rollModule.EightBall("test");

            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Contains("test") && flatResponses.Any(response => s.Contains(response))
            );
        }

        [Test]
        public async Task DiceRoll_WithInvalidNumber_DoesntRoll()
        {
            await rollModule.DiceRoll(0);

            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Equals("Please enter a positive number for the number of sides")
            );
        }

        [Test]
        public async Task DiceRoll_WithValidNumber_Rolls()
        {
            await rollModule.DiceRoll(6);

            VerifyHelper.VerifyInteractionAsync(mockContext, s => s.Contains("GlobalName rolled"));
        }

        [Test]
        public async Task CoinFlip_Flips()
        {
            await rollModule.CoinFlip();

            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Equals("GlobalName got *Heads*") || s.Equals("GlobalName got *Tails*")
            );
        }

        [Test]
        public async Task RollWaifu_RollSuccess_Posts()
        {
            await rollModule.RollWaifu();

            VerifyHelper.VerifyInteractionAsync(mockContext, s => s.Length > 0);
        }

        [Test]
        public async Task QuoteUser_WhenUserIsBot_ShouldNotQuote()
        {
            await rollModule.QuoteUser(mockBotUser.Object);
            VerifyHelper.VerifyInteractionAsync(
                mockContext,
                s => s.Equals("Can't quote bots :shrug:")
            );
        }
    }
}

using Discord.Commands;
using Discord;
using Feliciabot.net._6._0.commands.fun;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class ImageCommandTest
    {
        private readonly Mock<IMessageChannel> mockChannel;
        private readonly Mock<ICommandContext> mockContext;
        private readonly Mock<IRandomizerService> mockRandomizerService;
        private readonly ImageCommand imageCommand;

        public ImageCommandTest()
        {
            mockChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            mockRandomizerService = new Mock<IRandomizerService>();
            imageCommand = new ImageCommand(mockRandomizerService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockContext.SetupGet(c => c.Channel).Returns(mockChannel.Object);
            MockContextHelper.SetContext(imageCommand, mockContext.Object);
        }

        [Test]
        public async Task Awesome_PostsImage()
        {
            await imageCommand.Awesome();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\awesome.jpg"));
        }

        [Test]
        public async Task Banjo_PostsImage()
        {
            await imageCommand.Banjo();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\banjo.jpg"));
        }

        [Test]
        public async Task Bonk_PostsImage()
        {
            await imageCommand.Bonk();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\bonk.jpg"));
        }

        [Test]
        public async Task Buggin_PostsImage()
        {
            await imageCommand.Buggin();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\buggin.png"));
        }

        [Test]
        public async Task Cruel_PostsImage()
        {
            await imageCommand.Cruel();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\cruel.png"));
        }

        [Test]
        public async Task FireEmblem_PostsImage()
        {
            await imageCommand.FireEmblem();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\fireemblem.png"));
        }

        [Test]
        public async Task Home_PostsImage()
        {
            await imageCommand.Home();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\home.jpg"));
        }

        [Test]
        public async Task Pathetic_PostsImage()
        {
            await imageCommand.Pathetic();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\pathetic.png"));
        }

        [Test]
        public async Task Pog_PostsImage()
        {
            await imageCommand.Pog();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\pog.jpg"));
        }

        [Test]
        public async Task Regressing_PostsImage()
        {
            await imageCommand.Regressing();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\regressing.jpg"));
        }

        [Test]
        public async Task Shez_RandomIs0_PostsMaleImage()
        {
            mockRandomizerService.Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            await imageCommand.Shez();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\shez1.jpg"));
        }

        [Test]
        public async Task Shez_RandomIs1_PostsFemaleImage()
        {
            mockRandomizerService.Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            await imageCommand.Shez();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\shez2.jpg"));
        }

        [Test]
        public async Task Shock_PostsImage()
        {
            await imageCommand.Shock();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\shock.jpg"));
        }

        [Test]
        public async Task Stare_PostsImage()
        {
            await imageCommand.Stare();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\stare.gif"));
        }

        [Test]
        public async Task Stupid_PostsImage()
        {
            await imageCommand.Stupid();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\stupid.png"));
        }

        [Test]
        public async Task Xenoblade_PostsImage()
        {
            await imageCommand.Xenoblade();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"img\xenoblade.png"));
        }
    }
}

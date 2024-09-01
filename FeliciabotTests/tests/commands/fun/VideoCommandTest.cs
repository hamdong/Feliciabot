using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands.fun;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class VideoCommandTest
    {
        private readonly Mock<IMessageChannel> mockChannel;
        private readonly Mock<ICommandContext> mockContext;
        private readonly Mock<IRandomizerService> mockRandomizerService;
        private readonly VideoCommand videoCommand;

        public VideoCommandTest()
        {
            mockChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            mockRandomizerService = new Mock<IRandomizerService>();
            videoCommand = new VideoCommand(mockRandomizerService.Object);
        }

        [SetUp]
        public void Setup()
        {
            mockContext.SetupGet(c => c.Channel).Returns(mockChannel.Object);
            MockContextHelper.SetContext(videoCommand, mockContext.Object);
        }

        [Test]
        public async Task Alfred_PostsVideo()
        {
            await videoCommand.Alfred();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"videos\alfred.mov"));
        }

        [Test]
        public async Task GG_PostsVideo()
        {
            await videoCommand.GG();
            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals("https://www.youtube.com/watch?v=9nXYsmTv3Gg")
            );
        }

        [Test]
        public async Task Ganbare_PostsVideo()
        {
            await videoCommand.Ganbare();
            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals("https://www.youtube.com/watch?v=YoHq6DrWLSI")
            );
        }

        [Test]
        public async Task Huh_PostsVideo()
        {
            await videoCommand.Huh();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"videos\huh.mp4"));
        }

        [Test]
        public async Task Hi_PostsVideo()
        {
            await videoCommand.Hi();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"videos\video0.mov"));
        }

        [Test]
        public async Task Indeed_WhenHighRoll_PostsVideo()
        {
            mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(5);

            await videoCommand.Indeed();
            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals("https://www.youtube.com/watch?v=T5S4r2p9x34")
            );
        }

        [Test]
        public async Task Indeed_WhenLowRoll_PostsVideoAlt()
        {
            mockRandomizerService
                .Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(0);

            await videoCommand.Indeed();
            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals("https://www.youtube.com/watch?v=3fVo_Zy42FU")
            );
        }

        [Test]
        public async Task Shutup_PostsVideo()
        {
            await videoCommand.Shutup();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"\videos\shutup.mp4"));
        }

        [Test]
        public async Task Wahaha_PostsVideo()
        {
            await videoCommand.Wahaha();
            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s =>
                    s.Equals("https://www.youtube.com/watch?v=Q04Va_1JI04")
                    || s.Equals("https://www.youtube.com/shorts/hbFJSrx3sg0")
                    || s.Equals("https://www.youtube.com/watch?v=tjAsZ6bHCZE")
            );
        }

        [Test]
        public async Task What_PostsVideo()
        {
            await videoCommand.What();
            VerifyHelper.VerifyFileSentAsync(mockChannel, s => s.Contains(@"\videos\what.mov"));
        }
    }
}

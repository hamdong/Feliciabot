using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class VideoCommandTest
    {
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly VideoCommand _videoCommand;

        public VideoCommandTest()
        {
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _videoCommand = new VideoCommand();
        }

        [SetUp]
        public void Setup()
        {
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
            MockContextHelper.SetContext(_videoCommand, _mockContext.Object);
        }

        [Test]
        public async Task Alfred_PostsVideo()
        {
            await _videoCommand.Alfred();
            VerifySendFile(@"videos\alfred.mov");
        }

        [Test]
        public async Task GG_PostsVideo()
        {
            await _videoCommand.GG();
            VerifySendMessage("https://www.youtube.com/watch?v=9nXYsmTv3Gg");
        }

        [Test]
        public async Task Ganbare_PostsVideo()
        {
            await _videoCommand.Ganbare();
            VerifySendMessage("https://www.youtube.com/watch?v=YoHq6DrWLSI");
        }

        [Test]
        public async Task Huh_PostsVideo()
        {
            await _videoCommand.Huh();
            VerifySendFile(@"videos\huh.mp4");
        }

        [Test]
        public async Task Hi_PostsVideo()
        {
            await _videoCommand.Hi();
            VerifySendFile(@"videos\video0.mov");
        }

        private void VerifySendFile(string filePath)
        {
            _mockChannel.Verify(
                c =>
                    c.SendFileAsync(
                        It.Is<string>(s => s.Contains(filePath)),
                        null,
                        false,
                        null,
                        null,
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        MessageFlags.None,
                        null
                    ),
                Times.Once
            );
        }

        private void VerifySendMessage(string message)
        {
            _mockChannel.Verify(
                c =>
                    c.SendMessageAsync(
                        It.Is<string>(s => s.Equals(message)),
                        false,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        MessageFlags.None,
                        null
                    ),
                Times.Once
            );
        }
    }
}

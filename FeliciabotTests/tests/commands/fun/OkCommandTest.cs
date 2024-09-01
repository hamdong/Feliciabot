using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands.fun;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class OkCommandTest
    {
        private readonly Mock<IUser> mockCurrentUser;
        private readonly Mock<IUser> mockPreviousUser;
        private readonly Mock<IMessage> mockMessage;
        private readonly Mock<IUserMessage> mockUserMessage;
        private readonly Mock<IMessageChannel> mockChannel;
        private readonly Mock<ICommandContext> mockContext;
        private readonly OkCommand _okCommand;

        public OkCommandTest()
        {
            mockCurrentUser = new Mock<IUser>();
            mockPreviousUser = new Mock<IUser>();
            mockMessage = new Mock<IMessage>();
            mockUserMessage = new Mock<IUserMessage>();
            mockChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            _okCommand = new OkCommand();
        }

        [SetUp]
        public void Setup()
        {
            mockChannel.Reset();
            mockCurrentUser.SetupGet(u => u.Username).Returns("Current");
            mockPreviousUser.SetupGet(u => u.Username).Returns("Previous");
            mockMessage.SetupGet(m => m.Author).Returns(mockPreviousUser.Object);
            mockUserMessage.SetupGet(m => m.Author).Returns(mockCurrentUser.Object);

            var messageCollection = new List<IMessage>
            {
                mockMessage.Object,
                mockMessage.Object,
                mockMessage.Object,
            }.AsReadOnly();
            var asyncMessageCollection = GetAsAsyncEnumerable(messageCollection);
            mockChannel
                .Setup(c => c.GetMessagesAsync(It.IsAny<int>(), CacheMode.AllowDownload, null))
                .Returns(asyncMessageCollection);

            mockContext.SetupGet(c => c.Message).Returns(mockUserMessage.Object);
            mockContext.SetupGet(c => c.Channel).Returns(mockChannel.Object);
            mockContext.SetupGet(c => c.User).Returns(mockCurrentUser.Object);
            MockContextHelper.SetContext(_okCommand, mockContext.Object);
        }

        [Test]
        public async Task Ok_WhenNoUserAndDifferentAuthor_CallsOutLastUser()
        {
            await _okCommand.Ok();

            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals($"ok {mockPreviousUser.Object.Username}")
            );
        }

        [Test]
        public async Task Ok_WhenNoUserAndSameAuthor_CallsOutAuthorUser()
        {
            mockMessage.SetupGet(m => m.Author).Returns(mockCurrentUser.Object);

            await _okCommand.Ok();

            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals($"ok {mockCurrentUser.Object.Username}")
            );
        }

        [Test]
        public async Task Ok_WhenUser_CallsOutUser()
        {
            await _okCommand.Ok(mockCurrentUser.Object);

            VerifyHelper.VerifyMessageSentAsync(
                mockChannel,
                s => s.Equals($"ok {mockCurrentUser.Object.Username}")
            );
        }

        private static async IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetAsAsyncEnumerable(
            IReadOnlyCollection<IMessage> messages
        )
        {
            await Task.Delay(0);
            yield return messages;
        }
    }
}

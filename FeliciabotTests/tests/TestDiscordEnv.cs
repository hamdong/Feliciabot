using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.services.interfaces;
using Moq;

namespace FeliciabotTests.tests
{
    public class TestDiscordEnv
    {
        public ulong selfUserId = 9999;
        public ulong otherUserId = 1111;
        public readonly Mock<IDiscordClient> mockClient;
        public readonly Mock<ISelfUser> mockSelfUser;
        public readonly Mock<IUser> mockSelfUserAsIUser;
        public readonly Mock<IUser> mockUser;
        public readonly Mock<IUserMessage> mockUserMessage;
        public readonly Mock<IMessageChannel> mockMessageChannel;
        public readonly Mock<ICommandContext> mockContext;
        public readonly Mock<IRandomizerService> mockRandomizerService;
        public readonly Mock<MessageReference> mockMessageReference;

        public TestDiscordEnv()
        {
            mockClient = new Mock<IDiscordClient>();

            mockSelfUser = new Mock<ISelfUser>();
            mockSelfUserAsIUser = new Mock<IUser>();
            mockUser = new Mock<IUser>();
            mockUserMessage = new Mock<IUserMessage>();
            mockMessageChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            mockRandomizerService = new Mock<IRandomizerService>();
            mockMessageReference = new Mock<MessageReference>(
                null!,
                null!,
                null!,
                null!,
                MessageReferenceType.Default
            );

            mockMessageChannel.Reset();
            mockClient.SetupGet(c => c.CurrentUser).Returns(mockSelfUser.Object);

            mockSelfUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            mockSelfUser.SetupGet(u => u.Id).Returns(selfUserId);

            mockSelfUserAsIUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            mockSelfUserAsIUser.SetupGet(u => u.Id).Returns(selfUserId);

            mockUser.SetupGet(u => u.GlobalName).Returns("GlobalName");
            mockUser.SetupGet(u => u.Id).Returns(otherUserId);

            mockUserMessage.SetupGet(m => m.Channel).Returns(mockMessageChannel.Object);
            mockContext.SetupGet(c => c.Client).Returns(mockClient.Object);
            mockContext.SetupGet(c => c.Channel).Returns(mockMessageChannel.Object);
            mockContext.SetupGet(c => c.User).Returns(mockUser.Object);
        }
    }
}

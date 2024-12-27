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
        public ulong systemChannelId = 2222;
        public readonly Mock<IDiscordClient> mockClient;
        public readonly Mock<ISelfUser> mockSelfUser;
        public readonly Mock<IGuildUser> mockGuildUser;
        public readonly Mock<IGuild> mockGuild;
        public readonly Mock<ITextChannel> mockSystemChannel;
        public readonly Mock<IUser> mockSelfUserAsIUser;
        public readonly Mock<IUser> mockUser;
        public readonly Mock<IUserMessage> mockUserMessage;
        public readonly Mock<IMessageChannel> mockMessageChannel;
        public readonly Mock<ICommandContext> mockContext;
        public readonly Mock<MessageReference> mockMessageReference;

        public TestDiscordEnv()
        {
            mockClient = new Mock<IDiscordClient>();

            mockSelfUser = new Mock<ISelfUser>();
            mockSelfUserAsIUser = new Mock<IUser>();
            mockGuildUser = new Mock<IGuildUser>();
            mockGuild = new Mock<IGuild>();
            mockSystemChannel = new Mock<ITextChannel>();
            mockUser = new Mock<IUser>();
            mockUserMessage = new Mock<IUserMessage>();
            mockMessageChannel = new Mock<IMessageChannel>();
            mockContext = new Mock<ICommandContext>();
            mockMessageReference = new Mock<MessageReference>(
                null!,
                null!,
                null!,
                null!,
                MessageReferenceType.Default
            );

            mockClient.SetupGet(c => c.CurrentUser).Returns(mockSelfUser.Object);

            mockSelfUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            mockSelfUser.SetupGet(u => u.Id).Returns(selfUserId);

            mockSelfUserAsIUser.SetupGet(u => u.GlobalName).Returns("SelfGlobalName");
            mockSelfUserAsIUser.SetupGet(u => u.Id).Returns(selfUserId);

            mockUser.SetupGet(u => u.GlobalName).Returns("GlobalName");
            mockUser.SetupGet(u => u.Id).Returns(otherUserId);

            mockGuildUser.SetupGet(u => u.Mention).Returns("User Mention");

            ResetSystemChannel();

            mockUserMessage.SetupGet(m => m.Channel).Returns(mockMessageChannel.Object);
            mockContext.SetupGet(c => c.Client).Returns(mockClient.Object);
            mockContext.SetupGet(c => c.Channel).Returns(mockMessageChannel.Object);
            mockContext.SetupGet(c => c.User).Returns(mockUser.Object);
        }

        public void ResetSystemChannel()
        {
            mockSystemChannel.Reset();
            mockSystemChannel.SetupGet(c => c.Id).Returns(systemChannelId);
            mockSystemChannel.SetupGet(c => c.Name).Returns("System");
            mockSystemChannel.SetupGet(c => c.Guild).Returns(mockGuild.Object);
        }
    }
}

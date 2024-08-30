using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;
using System.Text;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class EmoteCommandTest
    {
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly EmoteCommand _emoteCommand;

        public EmoteCommandTest()
        {
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _emoteCommand = new EmoteCommand();
        }

        [SetUp]
        public void Setup()
        {
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
            MockContextHelper.SetContext(_emoteCommand, _mockContext.Object);
        }

        [Test]
        public async Task Civ_MessagesWithEmote()
        {
            await _emoteCommand.Civ();
            VerifySendMessage(EmoteCustom.FeliciaCiv);
        }

        [Test]
        public async Task Pad_MessagesWithEmote()
        {
            await _emoteCommand.Padoru();
            VerifySendMessage(EmoteCustom.Padoru);
        }

        [Test]
        public async Task Sip_MessagesWithEmote()
        {
            await _emoteCommand.Sip();
            VerifySendMessage(EmoteCustom.PyraSip);
        }

        [Test]
        public async Task Spin_MessagesWithEmote()
        {
            await _emoteCommand.Spin();
            VerifySendMessage(EmoteCustom.FeliciaSpin);
        }

        [Test]
        public async Task Clap_MessagesWithEmote()
        {
            await _emoteCommand.Clap();
            VerifySendMessage(EmoteCustom.WiiClap);
        }

        [Test]
        public async Task Clap_WithNumericInputLessThan12_MessagesWithMultipleEmotes()
        {
            var maxClaps = 6;
            StringBuilder sb = new();

            for (int i = 0; i < maxClaps; i++)
            {
                sb.Append(EmoteCustom.WiiClap);
            }

            await _emoteCommand.Clap(maxClaps);
            VerifySendMessage(sb.ToString());
        }

        [Test]
        public async Task Clap_WithNumericInputGreaterThan12_MessagesWith12Emotes()
        {
            var maxClaps = 13;
            StringBuilder sb = new();

            for (int i = 0; i < 12; i++)
            {
                sb.Append(EmoteCustom.WiiClap);
            }

            await _emoteCommand.Clap(maxClaps);
            VerifySendMessage(sb.ToString());
        }

        [Test]
        public async Task Pyradog_MessagesWithEmote()
        {
            await _emoteCommand.Pyradog();
            VerifySendMessage(WithPyraDog(EmoteCustom.Pyradog2));
        }

        [Test]
        public async Task Tatdog_MessagesWithEmote()
        {
            await _emoteCommand.Tatianadog();
            VerifySendMessage(WithPyraDog(EmoteCustom.Tatiana));
        }

        [Test]
        public async Task Aibadog_MessagesWithEmote()
        {
            await _emoteCommand.Aibadog();
            VerifySendMessage(WithPyraDog(EmoteCustom.Aiba));
        }

        [Test]
        public async Task Ninodog_MessagesWithEmote()
        {
            await _emoteCommand.Ninodog();
            VerifySendMessage(WithPyraDog(EmoteCustom.Nino));
        }

        [Test]
        public async Task Pogdog_MessagesWithEmote()
        {
            await _emoteCommand.Pogdog();
            VerifySendMessage(WithPyraDog(EmoteCustom.PyraPoggers));
        }

        [Test]
        public async Task Okudog_MessagesWithEmote()
        {
            await _emoteCommand.Okudog();
            VerifySendMessage(WithPyraDog(EmoteCustom.Oku));
        }

        [Test]
        public async Task CowboyNinodog_MessagesWithEmote()
        {
            await _emoteCommand.Cowboyninodog();
            VerifySendMessage(WithPyraDog(EmoteCustom.CowboyNino));
        }

        [Test]
        public async Task Shuffledog_MessagesWithEmote()
        {
            await _emoteCommand.Shuffledog();
            _mockChannel.Verify(
            c =>
                    c.SendMessageAsync(
                        It.IsAny<string>(),
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

        private static string WithPyraDog(string pyradogHead)
        {
            return $"{EmoteCustom.Pyradog1}{pyradogHead}{EmoteCustom.Pyradog3}\n" +
                $"{EmoteCustom.Pyradog4}{EmoteCustom.Pyradog5}{EmoteCustom.Pyradog6}\n" +
                $"{EmoteCustom.Pyradog7}{EmoteCustom.Pyradog8}{EmoteCustom.Pyradog9}";
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

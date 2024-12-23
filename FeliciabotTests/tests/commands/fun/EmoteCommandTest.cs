using System.Text;
using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class EmoteCommandTest
    {
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IRandomizerService> _mockRandomizerService;
        private readonly EmoteCommand _emoteCommand;

        public EmoteCommandTest()
        {
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _mockRandomizerService = new Mock<IRandomizerService>();
            _emoteCommand = new EmoteCommand(_mockRandomizerService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockChannel.Reset();
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
            MockContextHelper.SetContext(_emoteCommand, _mockContext.Object);
            _mockRandomizerService.Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>())).Returns(1);
        }

        [Test]
        public async Task Civ_MessagesWithEmote()
        {
            await _emoteCommand.Civ();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(EmoteCustom.FeliciaCiv)
            );
        }

        [Test]
        public async Task Pad_MessagesWithEmote()
        {
            await _emoteCommand.Padoru();
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals(EmoteCustom.Padoru));
        }

        [Test]
        public async Task Sip_MessagesWithEmote()
        {
            await _emoteCommand.Sip();
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals(EmoteCustom.PyraSip));
        }

        [Test]
        public async Task Spin_MessagesWithEmote()
        {
            await _emoteCommand.Spin();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(EmoteCustom.FeliciaSpin)
            );
        }

        [Test]
        public async Task Clap_MessagesWithEmote()
        {
            await _emoteCommand.Clap();
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals(EmoteCustom.WiiClap));
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
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals(sb.ToString()));
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
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals(sb.ToString()));
        }

        [Test]
        public async Task Pyradog_MessagesWithEmote()
        {
            await _emoteCommand.Pyradog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.Pyradog2))
            );
        }

        [Test]
        public async Task Tatdog_MessagesWithEmote()
        {
            await _emoteCommand.Tatianadog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.Tatiana))
            );
        }

        [Test]
        public async Task Aibadog_MessagesWithEmote()
        {
            await _emoteCommand.Aibadog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.Aiba))
            );
        }

        [Test]
        public async Task Ninodog_MessagesWithEmote()
        {
            await _emoteCommand.Ninodog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.Nino))
            );
        }

        [Test]
        public async Task Pogdog_MessagesWithEmote()
        {
            await _emoteCommand.Pogdog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.PyraPoggers))
            );
        }

        [Test]
        public async Task Okudog_MessagesWithEmote()
        {
            await _emoteCommand.Okudog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.Oku))
            );
        }

        [Test]
        public async Task CowboyNinodog_MessagesWithEmote()
        {
            await _emoteCommand.Cowboyninodog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals(WithPyraDog(EmoteCustom.CowboyNino))
            );
        }

        [Test]
        public async Task Shuffledog_MessagesWithEmote()
        {
            await _emoteCommand.Shuffledog();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s =>
                    s.Contains(EmoteCustom.Pyradog1)
                    && s.Contains(EmoteCustom.Pyradog2)
                    && s.Contains(EmoteCustom.Pyradog3)
                    && s.Contains(EmoteCustom.Pyradog4)
                    && s.Contains(EmoteCustom.Pyradog5)
                    && s.Contains(EmoteCustom.Pyradog6)
                    && s.Contains(EmoteCustom.Pyradog7)
                    && s.Contains(EmoteCustom.Pyradog8)
                    && s.Contains(EmoteCustom.Pyradog9)
            );
        }

        private static string WithPyraDog(string pyradogHead)
        {
            return $"{EmoteCustom.Pyradog1}{pyradogHead}{EmoteCustom.Pyradog3}\n"
                + $"{EmoteCustom.Pyradog4}{EmoteCustom.Pyradog5}{EmoteCustom.Pyradog6}\n"
                + $"{EmoteCustom.Pyradog7}{EmoteCustom.Pyradog8}{EmoteCustom.Pyradog9}";
        }
    }
}

using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;
using System.Text;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class EmoteCommandTest
    {
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IMessagingService> _mockMessagingService;
        private readonly EmoteCommand _emoteCommand;
        public EmoteCommandTest()
        {
            _mockContext = new Mock<ICommandContext>();
            _mockMessagingService = new Mock<IMessagingService>();
            _emoteCommand = new EmoteCommand(_mockMessagingService.Object);
        }

        [SetUp]
        public void Setup()
        {
            MockContextHelper.SetContext(_emoteCommand, _mockContext.Object);
            _mockMessagingService.Setup(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        [TearDown]
        public void Teardown()
        {
            _mockMessagingService.Reset();
        }

        [Test]
        public async Task Civ_MessagesWithEmote()
        {
            await _emoteCommand.Civ();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), EmoteCustom.FeliciaCiv), Times.Once);
        }

        [Test]
        public async Task Pad_MessagesWithEmote()
        {
            await _emoteCommand.Padoru();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), EmoteCustom.Padoru), Times.Once);
        }

        [Test]
        public async Task Sip_MessagesWithEmote()
        {
            await _emoteCommand.Sip();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), EmoteCustom.PyraSip), Times.Once);
        }

        [Test]
        public async Task Spin_MessagesWithEmote()
        {
            await _emoteCommand.Spin();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), EmoteCustom.FeliciaSpin), Times.Once);
        }

        [Test]
        public async Task Clap_MessagesWithEmote()
        {
            await _emoteCommand.Clap();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), EmoteCustom.WiiClap), Times.Once);
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
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), sb.ToString()), Times.Once);
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
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), sb.ToString()), Times.Once);
        }

        [Test]
        public async Task Pyradog_MessagesWithEmote()
        {
            await _emoteCommand.Pyradog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.Pyradog2)), Times.Once);
        }

        [Test]
        public async Task Tatdog_MessagesWithEmote()
        {
            await _emoteCommand.Tatianadog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.Tatiana)), Times.Once);
        }

        [Test]
        public async Task Aibadog_MessagesWithEmote()
        {
            await _emoteCommand.Aibadog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.Aiba)), Times.Once);
        }

        [Test]
        public async Task Ninodog_MessagesWithEmote()
        {
            await _emoteCommand.Ninodog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.Nino)), Times.Once);
        }

        [Test]
        public async Task Pogdog_MessagesWithEmote()
        {
            await _emoteCommand.Pogdog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.PyraPoggers)), Times.Once);
        }

        [Test]
        public async Task Okudog_MessagesWithEmote()
        {
            await _emoteCommand.Okudog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.Oku)), Times.Once);
        }

        [Test]
        public async Task CowboyNinodog_MessagesWithEmote()
        {
            await _emoteCommand.Cowboyninodog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), WithPyraDog(EmoteCustom.CowboyNino)), Times.Once);
        }

        [Test]
        public async Task Shuffledog_MessagesWithEmote()
        {
            await _emoteCommand.Shuffledog();
            _mockMessagingService.Verify(s => s.SendMessageToContextAsync(It.IsAny<ICommandContext>(), It.IsAny<string>()), Times.Once);
        }

        private static string WithPyraDog(string pyradogHead)
        {
            return $"{EmoteCustom.Pyradog1}{pyradogHead}{EmoteCustom.Pyradog3}\n" +
                $"{EmoteCustom.Pyradog4}{EmoteCustom.Pyradog5}{EmoteCustom.Pyradog6}\n" +
                $"{EmoteCustom.Pyradog7}{EmoteCustom.Pyradog8}{EmoteCustom.Pyradog9}";
        }
    }
}

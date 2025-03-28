﻿using Discord;
using Discord.Commands;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.commands.fun
{
    [TestFixture]
    public class TroubleCommandTest
    {
        private readonly Mock<IMessageChannel> _mockChannel;
        private readonly Mock<ICommandContext> _mockContext;
        private readonly Mock<IRandomizerService> _mockRandomizerService;
        private readonly TroubleCommand _troubleCommand;

        public TroubleCommandTest()
        {
            _mockChannel = new Mock<IMessageChannel>();
            _mockContext = new Mock<ICommandContext>();
            _mockRandomizerService = new Mock<IRandomizerService>();
            _troubleCommand = new TroubleCommand(_mockRandomizerService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _mockContext.SetupGet(c => c.Channel).Returns(_mockChannel.Object);
            MockContextHelper.SetContext(_troubleCommand, _mockContext.Object);
            _mockRandomizerService.Setup(s => s.GetRandom(It.IsAny<int>(), It.IsAny<int>())).Returns(100);
        }

        [Test]
        public async Task Trouble_MessagesThreeTimes()
        {
            await _troubleCommand.Trouble();
            VerifyHelper.VerifyMessageSentAsync(
                _mockChannel,
                s => s.Equals("WE'VE") || s.Equals("GOT") || s.Equals("TROUBLE!"),
                3
            );
        }

        [Test]
        public async Task Chairman_MessagesOnce()
        {
            await _troubleCommand.Chairman(false);
            VerifyHelper.VerifyMessageSentAsync(_mockChannel, s => s.Equals("bana"));
        }
    }
}

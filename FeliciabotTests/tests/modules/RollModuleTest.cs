using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Feliciabot.net._6._0.commands;
using Feliciabot.net._6._0.models;
using Feliciabot.net._6._0.modules;
using Feliciabot.net._6._0.services;
using Feliciabot.net._6._0.services.interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaifuSharp;

namespace FeliciabotTests.tests.modules
{
    [TestFixture]
    public class RollModuleTest
    {
        private readonly Mock<IInteractionContext> _mockContext;
        private readonly Mock<IInteractingService> _mockInteractingService;
        private readonly Mock<WaifuClient> _mockWaifuClient;
        private readonly RollModule _rollModule;

        public RollModuleTest()
        {
            _mockInteractingService = new Mock<IInteractingService>();
            _mockContext = new Mock<IInteractionContext>();
            _mockWaifuClient = new Mock<WaifuClient>();
            _rollModule = new RollModule(_mockWaifuClient.Object, _mockInteractingService.Object);
        }

        [SetUp]
        public void Setup()
        {
            TestCommandContext.SetContext(_rollModule, _mockContext.Object);
        }

        [Test]
        public async Task EightBall_RollsResponse()
        {
            var flatResponses = Roll.Responses.SelectMany(response => response);
            _mockInteractingService.Setup(s => s.SendRespondAsync(It.IsAny<SocketInteractionContext<SocketInteraction>>(), It.IsAny<string>()));

            await _rollModule.EightBall("test");

            _mockInteractingService.Verify(s => s.SendRespondAsync(It.IsAny<SocketInteractionContext<SocketInteraction>>(), It.Is<string>(s =>
                s.Contains("test") &&
                flatResponses.Any(response => s.Contains(response))
            )), Times.Once);
        }
    }
}

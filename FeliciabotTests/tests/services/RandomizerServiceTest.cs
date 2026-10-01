using Discord;
using Feliciabot.services;
using Moq;
using NUnit.Framework;

namespace FeliciabotTests.tests.services
{
    [TestFixture]
    public class RandomizerServiceTest
    {
        private readonly string expectedContent = "Content";
        private readonly string expectedUrl = "www.test.com";

        private readonly Mock<IAttachment> mockAttachment;
        private readonly Mock<IMessage> mockMessageWithAttachments;
        private readonly Mock<IMessage> mockMessageWithoutAttachments;

        private readonly RandomizerService randomizerService;

        public RandomizerServiceTest()
        {
            mockAttachment = new Mock<IAttachment>();
            mockMessageWithAttachments = new Mock<IMessage>();
            mockMessageWithoutAttachments = new Mock<IMessage>();

            randomizerService = new RandomizerService();
        }

        [SetUp]
        public void Setup()
        {
            mockMessageWithoutAttachments.Reset();
            mockMessageWithAttachments.Reset();
            mockAttachment.SetupGet(a => a.Url).Returns(expectedUrl);
            var attachments = new List<IAttachment> { mockAttachment.Object, mockAttachment.Object };
            mockMessageWithAttachments
                .SetupGet(m => m.Attachments)
                .Returns(attachments.AsReadOnly());
        }

        [Test]
        public void GetRandom_ShouldReturnValidNumber()
        {
            const int max = 100;
            const int min = 1;

            int result = randomizerService.GetRandom(min, max);

            Assert.That(result, Is.GreaterThanOrEqualTo(min));
            Assert.That(result, Is.LessThan(max));
        }

        [Test]
        public void GetRandom_WithSingleBound_ShouldReturnValueBelowExclusiveMaximum()
        {
            int result = randomizerService.GetRandom(1);

            Assert.That(result, Is.Zero);
        }

        [Test]
        public void GetRandomInclusive_ShouldReturnValueWithinInclusiveBounds()
        {
            const int min = 1;
            const int max = 6;

            int result = randomizerService.GetRandomInclusive(min, max);

            Assert.That(result, Is.InRange(min, max));
        }

        [Test]
        public void GetRandomInclusive_ShouldSupportIntMaxValueAsUpperBound()
        {
            int result = randomizerService.GetRandomInclusive(1, int.MaxValue);

            Assert.That(result, Is.InRange(1, int.MaxValue));
        }

        [Test]
        public void GetRandomAttachmentWithMessage_ShouldReturnContentAndAttachmentUrl()
        {
            mockMessageWithAttachments.SetupGet(m => m.Content).Returns(expectedContent);

            string result = randomizerService.GetRandomAttachmentWithMessageFromMessage(
                mockMessageWithAttachments.Object
            );

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo($"{expectedContent} {expectedUrl}"));
        }

        [Test]
        public void GetRandomAttachmentWithMessage_EmptyContent_ShouldReturnAttachmentUrl()
        {
            mockMessageWithAttachments.SetupGet(m => m.Content).Returns("");

            string result = randomizerService.GetRandomAttachmentWithMessageFromMessage(
                mockMessageWithAttachments.Object
            );

            Assert.That(result, Is.EqualTo(expectedUrl));
        }

        [Test]
        public void GetRandomAttachmentWithMessage_NoAttachments_ShouldReturnContent()
        {
            mockMessageWithoutAttachments.SetupGet(m => m.Content).Returns(expectedContent);
            mockMessageWithoutAttachments.SetupGet(m => m.Attachments).Returns([]);

            string result = randomizerService.GetRandomAttachmentWithMessageFromMessage(
                mockMessageWithoutAttachments.Object
            );

            Assert.That(result, Is.EqualTo(expectedContent));
        }

        [Test]
        public void GetRandomAttachmentWithMessage_NoAttachmentsOrContent_ShouldReturnErrorMessage()
        {
            mockMessageWithoutAttachments.SetupGet(m => m.Content).Returns("");
            mockMessageWithoutAttachments.SetupGet(m => m.Attachments).Returns([]);

            string result = randomizerService.GetRandomAttachmentWithMessageFromMessage(
                mockMessageWithoutAttachments.Object
            );

            Assert.That(result, Is.EqualTo("Couldn't find a message :confused:"));
        }
    }
}

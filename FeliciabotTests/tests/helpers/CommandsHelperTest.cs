using Feliciabot.net._6._0.helpers;
using NUnit.Framework;

namespace FeliciabotTests.tests.helpers
{
    [TestFixture]
    public class CommandsHelperTest
    {
        [Test]
        public void Does_Testing_Work()
        {
            Assert.That(true);
        }

        [Test]
        public void IsNonCommandQuery_Returns_True_For_Normal_String()
        {
            Assert.That(CommandsHelper.IsNonCommandQuery("Hello world"));
        }

        [Test]
        public void IsNonCommandQuery_Returns_False_For_Command_Query_String()
        {
            Assert.That(CommandsHelper.IsNonCommandQuery(""), Is.False);
            Assert.That(CommandsHelper.IsNonCommandQuery("!Hello world"), Is.False);
            Assert.That(CommandsHelper.IsNonCommandQuery("feh!Hello world"), Is.False);
            Assert.That(CommandsHelper.IsNonCommandQuery("@Hello world"), Is.False);
            Assert.That(CommandsHelper.IsNonCommandQuery(".Hello world"), Is.False);
        }

        [Test]
        public void RandomNumber_Returns_Number_In_Range()
        {
            int num = CommandsHelper.GetRandomNumber(5, 1);
            Assert.That(num >= 1 && num <= 5);
        }
    }
}

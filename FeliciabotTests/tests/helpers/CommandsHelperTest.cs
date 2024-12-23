using Feliciabot.net._6._0.helpers;
using NUnit.Framework;

namespace FeliciabotTests.tests.helpers
{
    [TestFixture]
    public class CommandsHelperTest
    {
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
    }
}

using NUnit.Framework;
using Feliciabot.net._6._0.helpers;

namespace FeliciabotTests.tests
{
    [TestFixture]
    public class CommandsHelperTest
    {
        [Test]
        public void Does_Testing_Work()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void GetStringArrayFromFile_Returns_String_Array()
        {
            string path = Environment.CurrentDirectory + @"\data\greetings.txt";
            var stringArray = CommandsHelper.GetStringArrayFromFile(path);
            Assert.IsTrue(stringArray.GetType().Name == "String[]");
        }

        [Test]
        public void IsNonCommandQuery_Returns_True_For_Normal_String()
        {
            Assert.IsTrue(CommandsHelper.IsNonCommandQuery("Hello world"));
        }

        [Test]
        public void IsNonCommandQuery_Returns_False_For_Command_Query_String()
        {
            Assert.IsFalse(CommandsHelper.IsNonCommandQuery(""));
            Assert.IsFalse(CommandsHelper.IsNonCommandQuery("!Hello world"));
            Assert.IsFalse(CommandsHelper.IsNonCommandQuery("feh!Hello world"));
            Assert.IsFalse(CommandsHelper.IsNonCommandQuery("@Hello world"));
            Assert.IsFalse(CommandsHelper.IsNonCommandQuery(".Hello world"));
        }

        [Test]
        public void RandomNumber_Returns_Number_In_Range()
        {
            int num = CommandsHelper.GetRandomNumber(5, 1);
            Assert.IsTrue(num >= 1 && num <= 5);
        }
    }
}

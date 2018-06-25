using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentConsole.CommandResults;

namespace ContentConsole.Tests
{
    [TestClass]
    public class CommandProcessor_Tests
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly string[] _initial_bad_words = new string[] { "bad", "horrible" };
        private readonly string _example_text = @"The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.";

        private ICommandResult GetText(string[] args)
        {
            return new CommandResultWithText(_example_text);
        }

        private ICommandResult GetWords(string[] args)
        {
            return new CommandResultWithText(string.Join(" ", _initial_bad_words));
        }

        private ICommandResult GetNewWords(string[] args)
        {
            return new CommandResultWithText("worse awful disgusting bad");
        }

        [TestMethod]
        public void GoAsUser_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { "u" });
            Assert.Fail();
        }

        [TestMethod]
        public void GoAsAdmin_DictonaryChanged()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void GoAsReader_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { "r" });
            Assert.Fail();
        }
        [TestMethod]
        public void GoAsContentCurator_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { "c" });
            Assert.Fail();
        }
    }
}

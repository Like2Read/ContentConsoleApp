using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentConsole.CommandResults;
using DataControl;
using DataControl.Mock;
using System.Collections.Generic;

namespace ContentConsole.Tests
{
    [TestClass]
    public class CommandProcessor_Tests
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly string[] _initial_bad_words = new string[] { "bad", "horrible" };
        private readonly string[] _initial_bad_words_duplicated = new string[] { "bad", "horrible", "bad", "horrible" };
        private readonly string[] _new_bad_words = new string[] { "worse", "awful", "disgusting", "bad" };
        private readonly string _example_text = @"The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.";
        private readonly IDataControllerFactory _factory = new DataControllerFactory();

        public CommandProcessor_Tests()
        {
            _commandProcessor = new CommandProcessor(_factory.Create(_initial_bad_words.ToArray()));
        }

        private ICommandResult GetText(string[] args)
        {
            return new CommandResultWithText(_example_text);
        }

        private ICommandResult GetWords(string[] args)
        {
            return new CommandResultWithText(string.Join(" ", _initial_bad_words));
        }

        private ICommandResult GetDuplicatedWords(string[] args)
        {
            return new CommandResultWithText(string.Join(" ", _initial_bad_words_duplicated));
        }

        private ICommandResult GetNewWords(string[] args)
        {
            return new CommandResultWithText(string.Join(" ", _new_bad_words));
        }

        [TestMethod]
        public void GoAsUser_ReturnsOK()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { UserCommands.User }, GetText, GetWords);
            Assert.IsTrue(result.OK);
        }

        [TestMethod]
        public void GoAsUser_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { UserCommands.User }, GetText, GetWords);
            var writer = new StringWriter();
            result.Print(writer);
            var resultString = writer.ToString();
            Assert.AreEqual(@"Scanned the text:
The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.
Total Number of negative words: 2
", resultString);
        }

        [TestMethod]
        public void GoAsAdmin_DictonaryChanged()
        {
            var localCommandProcessor = new CommandProcessor(_factory.Create(_initial_bad_words.ToArray()));
            var result = localCommandProcessor.ProcessCommand(new string[] { UserCommands.Administrator }, GetText, GetNewWords);
            var writer = new StringWriter();
            result.Print(writer);
            var resultString = writer.ToString();
            Assert.AreEqual(@"There were 2 bad words in the dictionary:
bad horrible
Dictionary was updated
There are 4 bad words in the dictionary:
worse awful disgusting bad
", resultString);
        }

        [TestMethod]
        public void GoAsAdmin_WithDuplicatesInDictionaryTheResultIsTheSame()
        {
            var localCommandProcessor = new CommandProcessor(_factory.Create(_initial_bad_words.ToArray()));
            var resultWithNoDuplicatedDictionary = localCommandProcessor.ProcessCommand(new string[] { UserCommands.User }, GetText, GetWords);

            var result = localCommandProcessor.ProcessCommand(new string[] { UserCommands.Administrator }, GetText, GetDuplicatedWords);
            var resultWithDuplicatedDictionary = localCommandProcessor.ProcessCommand(new string[] { UserCommands.User }, GetText, GetWords);

            Assert.AreEqual(resultWithDuplicatedDictionary.Message, resultWithNoDuplicatedDictionary.Message);
        }

        [TestMethod]
        public void GoAsReader_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { UserCommands.Reader }, GetText, GetWords);
            var writer = new StringWriter();
            result.Print(writer);
            var resultString = writer.ToString();
            Assert.AreEqual(@"Text was edited. Bad words were removed.
The weather in Manchester in winter is b#d. It rains all the time - it must be h######e for people visiting.
", resultString);
        }
        [TestMethod]
        public void GoAsContentCurator_TextFromContentStory()
        {
            var result = _commandProcessor.ProcessCommand(new string[] { UserCommands.ContentCurator }, GetText, GetWords);
            var writer = new StringWriter();
            result.Print(writer);
            var resultString = writer.ToString();
            Assert.AreEqual(@"Total Number of negative words: 2
Unfiltered text:
The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.
", resultString);
        }
    }
}

using System.IO;
using System.Linq;
using ContentConsole.CommandResults;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentConsole.Tests
{
    [TestClass]
    public class InputTextController_Tests
    {
        [TestMethod]
        public void TryReadWords_FileDoesNotExist()
        {
            var result = InputTextController.TryReadWords("", out string[] words);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryReadWords_WordsReadFromFilesFolder()
        {
            InputTextController.TryReadWords(@"Files\dictionary.txt", out string[] words);
            CollectionAssert.AreEqual(new string[] { "bad", "horrible" }, words.OrderBy(word => word).ToArray());
        }

        [TestMethod]
        public void ReadTextFromFile_NullInParameters_NoFile()
        {
            var result = InputTextController.ReadTextFromFile(null, File.ReadAllText);
            Assert.AreEqual("No file was provided", result.Message);
        }

        [TestMethod]
        public void ReadTextFromFile_LessThan2InParameters_NoFile()
        {
            var result = InputTextController.ReadTextFromFile(new string[] { "dictionary.txt" }, File.ReadAllText);
            Assert.AreEqual("No file was provided", result.Message);
        }

        [TestMethod]
        public void ReadTextFromFile_FileDoesNotExist()
        {
            var fileName = "unknown.txt";
            var result = InputTextController.ReadTextFromFile(new string[] { "a", fileName }, File.ReadAllText);
            Assert.AreEqual($"File {fileName} was not found in Files folder", result.Message);
        }

        [TestMethod]
        public void ReadTextFromFile_SuccesfullyReadFromDictionaryTxt()
        {
            var fileName = "original.txt";
            var result = InputTextController.ReadTextFromFile(new string[] { "a", fileName }, File.ReadAllText) as CommandResultWithText;
            Assert.AreEqual("The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.", result.Text);
        }

    }
}

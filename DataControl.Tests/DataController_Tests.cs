using System.Linq;
using DataControl.Mock;
using DataControl.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DataControl.Tests
{
    [TestClass]
    public class DataController_Tests
    {
        private readonly DataController _bad_horrible_controller;
        private readonly string[] _initial_bad_words = new string[] { "bad", "horrible" };
        private readonly string _example_text = @"The weather in Manchester in winter is bad. It rains all the time - it must be horrible for people visiting.";

        public DataController_Tests()
        {
            var mock_repository = new Mock<IWordsRepository>();
            mock_repository.Setup(m => m.GetAllWords()).Returns(_initial_bad_words.ToArray());
            _bad_horrible_controller = new DataController(mock_repository.Object);
        }

        [TestMethod]
        public void ScanTextForWords_ReturnsScanResult()
        {
            var result = _bad_horrible_controller.ScanTextForWords("");

            Assert.IsTrue(result is ScanResult);
        }

        [TestMethod]
        public void ScanTextForWords_Find2BadWordsInTheTextFromTheStory()
        {
            var result = _bad_horrible_controller.ScanTextForWords(_example_text) as ScanResult;
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void ScanTextForWords_Find3BadWordsOneIsDuplicated()
        {
            var result = _bad_horrible_controller.ScanTextForWords("It was a bad day. Not just bad, it was horrible.") as ScanResult;
            Assert.AreEqual(result.Count, 3);
        }

        [TestMethod]
        public void ScanTextForWords_Find2BadWordsInTheTextFromTheStoryUpperCaseIgnored()
        {
            var result = _bad_horrible_controller.ScanTextForWords(_example_text.ToUpperInvariant()) as ScanResult;
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void ScanTextForWords_Find1BadWordsInTheText()
        {
            var result = _bad_horrible_controller.ScanTextForWords("It was a bad day") as ScanResult;
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void ScanTextForWords_Find0BadWordsInTheEmptyText()
        {
            var result = _bad_horrible_controller.ScanTextForWords("") as ScanResult;
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void ScanTextForWords_Find2BadWordsInTheTextWithQuestionSignCommaAndBrackets()
        {
            var result = _bad_horrible_controller.ScanTextForWords("Was this day [bad? Or was it not that (horrible,what do you think?)") as ScanResult;
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void GetAllWords_ResultIsTheSameAsInitialWords()
        {
            var result = _bad_horrible_controller.GetAllWords();
            CollectionAssert.AreEqual(_initial_bad_words, result.ToArray());
        }

        [TestMethod]
        public void SetWords_ReturnsUpdateResult()
        {
            var mock_repository = new Mock<IWordsRepository>();
            mock_repository.Setup(m => m.GetAllWords()).Returns(_initial_bad_words.ToArray());
            var controller = new DataController(mock_repository.Object);
            var result = controller.SetWords(new string[] { "terrible" });

            Assert.IsTrue(result is UpdateResult);
        }

        [TestMethod]
        public void SetWords_ReturnsTheSameAsNewWords()
        {
            var newWords = new string[] { "terrible" };
            var mock_repository = new MoqWordsRepository(new string[] { "bad", "horrible" });
            var controller = new DataController(mock_repository);
            controller.SetWords(newWords);

            var words = controller.GetAllWords();
            CollectionAssert.AreEqual(words.ToArray(), newWords);
        }

        [TestMethod]
        public void SetWords_CheckScanResultsBeforeAndAfter()
        {
            var newWords = new string[] { "terrible" };
            var mock_repository = new MoqWordsRepository(new string[] { "bad", "horrible" });
            var controller = new DataController(mock_repository);

            var resultBefore = controller.ScanTextForWords(_example_text) as ScanResult;
            controller.SetWords(newWords);
            var resultAfter = controller.ScanTextForWords(_example_text) as ScanResult;

            Assert.AreNotEqual(resultBefore.Count, resultAfter.Count);
        }

        [TestMethod]
        public void SetWords_TryToSetNull()
        {
            var mock_repository = new Mock<IWordsRepository>();
            mock_repository.Setup(m => m.GetAllWords()).Returns(_initial_bad_words.ToArray());
            var controller = new DataController(mock_repository.Object);
            var result = controller.SetWords(null) as UpdateResult;

            Assert.AreEqual(result.Message, Messages.WordsAreNull);
        }

        [TestMethod]
        public void SetWords_TryToSetArrayWithNullAndEmptyWordsTheyAreIgnored()
        {
            var mock_repository = new MoqWordsRepository(new string[] { "bad", "horrible" });
            var controller = new DataController(mock_repository);

            var result = controller.SetWords(new string[] { null, "", "terrible", "worse" }) as UpdateResult;
            var newWords = controller.GetAllWords();

            CollectionAssert.AreEqual(new string[] { "terrible", "worse" }, newWords.OrderBy(x => x).ToArray());
        }

        [TestMethod]
        public void HideWords_BadWordsInStoryTextAreHidden()
        {
            _bad_horrible_controller.TryHideWords(_example_text, out string edited_text);
            Assert.AreEqual("The weather in Manchester in winter is b#d. It rains all the time - it must be h######e for people visiting.",
                edited_text);
        }

        [TestMethod]
        public void HideWords_WordsSimilarToBadWordsStayVisible()
        {
            var text = "Badler was not that bad";
            _bad_horrible_controller.TryHideWords(text, out string edited_text);
            Assert.AreEqual("Badler was not that b#d",
                edited_text);
        }

        [TestMethod]
        public void HideWords_WordsWithLengthLessThan3ReplacedWithHashes()
        {
            var mock_repository = new Mock<IWordsRepository>();
            mock_repository.Setup(m => m.GetAllWords()).Returns(new string[] { "no", "x" });
            var controller = new DataController(mock_repository.Object);

            var text = "The first letter is no X - that's for sure";
            controller.TryHideWords(text, out string edited_text);
            Assert.AreEqual("The first letter is ## # - that's for sure",
                edited_text);
        }

        [TestMethod]
        public void HideWordss_Hide2BadWordsInTheTextWithQuestionSignCommaAndBrackets()
        {
            var text = "Was this day [bad? Or was it not that (horrible,what do you think?)";
            _bad_horrible_controller.TryHideWords(text, out string edited_text);
            Assert.AreEqual("Was this day [b#d? Or was it not that (h######e,what do you think?)", edited_text);
        }
    }
}

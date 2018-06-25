using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataControl.Results;

namespace DataControl
{
    public class DataController: IDataController
    {
        private readonly IWordsRepository _repository;

        public DataController(IWordsRepository repository)
        {
            _repository = repository;
        }

        private MatchCollection GetMatches(IEnumerable<string> words, string text)
        {
            var escapedAndBoundedWords = words.Select(word => @"\b" + Regex.Escape(word) + @"\b");
            var pattern = new Regex("(" + string.Join(")|(", escapedAndBoundedWords) + ")", RegexOptions.IgnoreCase);

            return pattern.Matches(text);
        }

        public IResult ScanTextForWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new ScanResult(0);

            return new ScanResult(GetMatches(_repository.GetAllWords(), text).Count);
        }

        public IEnumerable<string> GetAllWords()
        {
            return _repository.GetAllWords();
        }

        public IResult SetWords(string[] words)
        {
            if (words == null)
                return new UpdateResult(false, Messages.WordsAreNull);

            var res = _repository.TrySetWords(words.Where(w => !string.IsNullOrEmpty(w)));
            return new UpdateResult(res, res ? Messages.DictionaryWasUpdated : Messages.FailedToUpdate);
        }

        public bool TryHideWords(string text, out string resultText)
        {
            resultText = text;
            if (string.IsNullOrEmpty(text))
                return false;

            var matches = GetMatches(_repository.GetAllWords(), text).Cast<Match>();

            foreach (var match in matches)
            {
                var word = match.Value;
                var pattern = @"\b" + word + @"\b";
                string replacement = (word.Length > 2) ? string.Concat(word[0], new string('#', word.Length - 2), word[word.Length - 1]) : new string('#', word.Length);
                resultText = Regex.Replace(resultText, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return true;
        }

    }
}

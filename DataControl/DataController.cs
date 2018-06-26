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

        [Obsolete]
        private MatchCollection GetMatches(IEnumerable<string> words, string text)
        {
            var escapedAndBoundedWords = words.Select(word => @"\b" + Regex.Escape(word) + @"\b");

            var pattern = new Regex("(" + string.Join(")|(", escapedAndBoundedWords) + ")", RegexOptions.IgnoreCase);

            return pattern.Matches(text);
        }

        private IEnumerable<string> GetMatches(string text)
        {
            var pattern = new Regex(@"\W");
            return pattern.Split(text).Where(word => _repository.ContainsWord(word));
        }

        public IResult ScanTextForWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new ScanResult(0);

            return new ScanResult(GetMatches(text).Count());
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

            var matches = GetMatches(text).Distinct();

            foreach (var word in matches)
            {
                var pattern = @"\b" + word + @"\b";
                string replacement = (word.Length > 2) ? string.Concat(word[0], new string('#', word.Length - 2), word[word.Length - 1]) : new string('#', word.Length);
                resultText = Regex.Replace(resultText, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return true;
        }

    }
}

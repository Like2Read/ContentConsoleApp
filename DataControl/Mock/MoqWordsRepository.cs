using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl.Mock
{
    public class MoqWordsRepository : IWordsRepository
    {
        private HashSet<string> _dictionary;

        public MoqWordsRepository(string[] words)
        {
            _dictionary = words != null && words.Length > 0 ? new HashSet<string>(words) : new HashSet<string>();
        }

        public bool ContainsWord(string word)
        {
            return _dictionary.Contains(word, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetAllWords()
        {
            return _dictionary.ToArray();
        }

        public bool TryAddWord(string word)
        {
            if (_dictionary.Contains(word, StringComparer.OrdinalIgnoreCase))
                return false;
            _dictionary.Add(word.ToLowerInvariant());
            return true;
        }

        public bool TryRemoveWord(string word)
        {
            if (!_dictionary.Contains(word, StringComparer.OrdinalIgnoreCase))
                return false;
            _dictionary.Remove(word.ToLowerInvariant());
            return true;
        }

        public bool TrySetWords(IEnumerable<string> words)
        {
            _dictionary = new HashSet<string>(words);
            return true;
        }
    }
}

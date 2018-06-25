using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl
{
    public interface IWordsRepository
    {
        bool TryAddWord(string word);
        bool AddWords(IEnumerable<string> words, out IEnumerable<string> skipped);
        bool ContainsWord(string word);
        bool TryRemoveWord(string word);
        IEnumerable<string> GetAllWords();
        bool TrySetWords(IEnumerable<string> words);
    }
}

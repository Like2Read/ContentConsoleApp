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

        public IResult ScanTextForWords(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new ScanResult(0);

            var escapedAndBoundedWords = _repository.GetAllWords().Select(word => @"\b" + Regex.Escape(word) + @"\b");
            var pattern = new Regex("(" + string.Join(")|(", escapedAndBoundedWords) + ")");

            return new ScanResult(pattern.Matches(text).Count);
        }
    }
}

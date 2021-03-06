﻿using DataControl.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl
{
    public interface IDataController
    {
        IResult ScanTextForWords(string text);
        IEnumerable<string> GetAllWords();
        IResult SetWords(string[] words);
        bool TryHideWords(string text, out string resultText);
    }
}

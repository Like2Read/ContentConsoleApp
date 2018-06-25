using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl.Results
{
    public class ScanResult : IResult
    {
        public int Count { get; }
        public ScanResult(int count)
        {
            Count = count;
        }

        public string Message => $"Total Number of negative words: {Count}";
    }
}

using DataControl.Results;
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
    }
}

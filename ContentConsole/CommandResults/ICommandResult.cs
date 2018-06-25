using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentConsole.CommandResults
{
    public interface ICommandResult
    {
        bool OK { get; }
        string Message { get; }

    }
}

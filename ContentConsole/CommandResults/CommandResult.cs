using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentConsole.CommandResults
{
    public class CommandResult : ICommandResult
    {
        public bool OK { get; }
        public string Message { get; }

        public CommandResult(bool ok, string message)
        {
            OK = ok;
            Message = message;
        }
    }
}

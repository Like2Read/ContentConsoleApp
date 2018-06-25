using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentConsole.CommandResults
{
    public class CommandResultWithText : ICommandResult
    {
        public bool OK => true;

        public string Message => "Text was edited. Bad words were removed.";

        public string Text { get; }

        public CommandResultWithText(string text)
        {
            Text = text;
        }
    }
}

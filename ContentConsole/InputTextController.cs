using ContentConsole.CommandResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentConsole
{
    public static class InputTextController
    {
        public static bool TryReadWords(string path, out string[] words)
        {
            words = null;
            if (!File.Exists(path))
            {
                return false;
            }

            words = File.ReadAllLines(path).Where(x => x.Trim().Length > 0).ToArray();

            return true;
        }

        public static ICommandResult ReadTextFromFile(string[] args, Func<string, string> readerFunction)
        {
            if (args == null || args.Length < 2)
            {
                return new CommandResult(false, "No file was provided");
            }
            var path = Path.Combine("Files", args[1]);

            if (!File.Exists(path))
                return new CommandResult(false, $"File {args[1]} was not found in Files folder");


            return new CommandResultWithText(readerFunction(path));
        }
/*
        public static ICommandResult ReadWordsFromFile(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                return new CommandResult(false, "No file with words was provided");
            }
            var path = Path.Combine("Files", args[1]);

            if (!TryReadWords(path, out string[] newWords))
            {
                return new CommandResult(false, $"File {args[1]} was not found in Files folder");
            }

            return new CommandResultWithText(string.Join(" ", newWords));
        }

        public static ICommandResult ReadTextFromFile(string[] args)
        {
            if (args.Length < 2)
            {
                return new CommandResult(false, "No file with text was provided");
            }
            var path = Path.Combine("Files", args[1]);
            if (!File.Exists(path))
                return new CommandResult(false, $"File {args[1]} was not found in Files folder");

            return new CommandResultWithText(File.ReadAllText(path));
        }
*/

    }
}

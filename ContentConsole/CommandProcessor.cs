using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentConsole.CommandResults;

namespace ContentConsole
{
    public class CommandProcessor
    {
        private readonly Func<string[], ICommandResult> _loadText;
        private readonly Func<string[], ICommandResult> _loadWords;

        private static Dictionary<string, Func<string[], ICommandResult>> _userCommands = new Dictionary<string, Func<string[], ICommandResult>>()
        {
        };

        public CommandProcessor(Func<string[], ICommandResult> loadText, Func<string[], ICommandResult> loadWords)
        {
            _loadText = loadText;
            _loadWords = loadWords;
        }

        public ICommandResult ProcessCommand(string[] args)
        {
            if (args.Length < 1)
                return new CommandResult(false, "No command was passed");

            if (!_userCommands.TryGetValue(args[0], out Func<string[], ICommandResult> goWithRole))
                return new CommandResult(false, "Unknown command");

            return goWithRole(args);
        }

        private ICommandResult GoAsAdmin(string newWords)
        {
            var arrayOfWords = newWords.Split(' ').Where(word => !string.IsNullOrEmpty(word)).Select(word => word.ToLowerInvariant()).ToArray();
            throw new NotImplementedException(nameof(GoAsAdmin));
        }

        private ICommandResult GoAsUser(string[] text)
        {
            throw new NotImplementedException(nameof(GoAsUser));
        }

        private ICommandResult GoAsReader(string text)
        {
            throw new NotImplementedException(nameof(GoAsReader));
        }

        private ICommandResult GoAsContentCurator(string text)
        {
            throw new NotImplementedException(nameof(GoAsContentCurator));
        }
    }
}

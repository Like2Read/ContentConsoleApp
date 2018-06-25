using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentConsole.CommandResults;
using DataControl;

namespace ContentConsole
{
    public class CommandProcessor
    {
        private readonly Func<string[], ICommandResult> _loadText;
        private readonly Func<string[], ICommandResult> _loadWords;
        private Dictionary<string, Func<string[], ICommandResult>> _userCommands;
        private readonly IDataController _controller;

        public CommandProcessor(IDataController controller, Func<string[], ICommandResult> loadText, Func<string[], ICommandResult> loadWords)
        {
            _controller = controller;
            _loadText = loadText;
            _loadWords = loadWords;

            _userCommands = new Dictionary<string, Func<string[], ICommandResult>>()
                            {
                                { "u", args => CommandExecutor(args, _loadText, GoAsUser)},
                                { "a", args => CommandExecutor(args, _loadWords, GoAsAdmin)},
                                { "r", args => CommandExecutor(args, _loadText, GoAsReader)},
                                { "c", args => CommandExecutor(args, _loadText, GoAsContentCurator)},
                            };
        }

        private static ICommandResult CommandExecutor(string[] args, Func<string[], ICommandResult> load, Func<string, ICommandResult> goWithRole)
        {
            ICommandResult result;
            return (!(result = load(args)).OK) ? result : goWithRole(((CommandResultWithText)result).Text);
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

        private ICommandResult GoAsUser(string text)
        {
            var result = _controller.ScanTextForWords(text);
            var builder = new StringBuilder();
            builder.AppendLine("Scanned the text:");
            builder.AppendLine(text);
            builder.AppendLine(result.Message);

            return new CommandResult(true, builder.ToString());
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

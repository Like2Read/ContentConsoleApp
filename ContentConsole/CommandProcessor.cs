using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentConsole.CommandResults;
using DataControl;
using DataControl.Results;

namespace ContentConsole
{
    public delegate ICommandResult LoadFunctionDelegate(string[] args);

    public class CommandProcessor
    {
        private Dictionary<string, Func<string[], LoadFunctionDelegate, ICommandResult>> _userCommands;
        private readonly IDataController _controller;

        public CommandProcessor(IDataController controller)
        {
            _controller = controller;

            _userCommands = new Dictionary<string, Func<string[], LoadFunctionDelegate, ICommandResult>>()
                            {
                                { UserCommands.User, (args, load) => CommandExecutor(args, load, GoAsUser)},
                                { UserCommands.Administrator, (args, load) => CommandExecutor(args, load, GoAsAdmin)},
                                { UserCommands.Reader, (args, load) => CommandExecutor(args, load, GoAsReader)},
                                { UserCommands.ContentCurator, (args, load) => CommandExecutor(args, load, GoAsContentCurator)},
                            };
        }

        private static ICommandResult CommandExecutor(string[] args, LoadFunctionDelegate load, Func<string, ICommandResult> goWithRole)
        {
            ICommandResult result;
            return (!(result = load(args)).OK) ? result : goWithRole(((CommandResultWithText)result).Text);
        }

        public ICommandResult ProcessCommand(string[] args, LoadFunctionDelegate load)
        {
            if (args.Length < 1)
                return new CommandResult(false, "No command was passed");

            if (!_userCommands.TryGetValue(args[0], out Func<string[], LoadFunctionDelegate, ICommandResult> goWithRole))
                return new CommandResult(false, "Unknown command");

            return goWithRole(args, load);
        }

        private ICommandResult GoAsAdmin(string newWords)
        {
            var arrayOfWords = newWords.Split(' ').Where(word => !string.IsNullOrEmpty(word)).Select(word => word.ToLowerInvariant()).ToArray();

            var words = _controller.GetAllWords().ToList();
            var builder = new StringBuilder();
            builder.AppendLine($"There were {words.Count} bad words in the dictionary:");
            builder.AppendLine(string.Join(" ", words));

            var result = _controller.SetWords(arrayOfWords) as UpdateResult;
            if (result.Success)
            {
                builder.AppendLine(result.Message);
                words = _controller.GetAllWords().ToList();
                builder.AppendLine($"There are {words.Count} bad words in the dictionary:");
                builder.AppendLine(string.Join(" ", words));

                return new CommandResult(true, builder.ToString());
            }

            return new CommandResult(false, result.Message);
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

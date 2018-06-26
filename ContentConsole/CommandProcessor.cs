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
    public delegate ICommandResult GoAsDelegate(string[] args, LoadFunctionDelegate loadText, LoadFunctionDelegate loadWords);

    public class CommandProcessor
    {
        private Dictionary<string, GoAsDelegate> _userCommands;
        private readonly IDataController _controller;

        public CommandProcessor(IDataController controller)
        {
            _controller = controller;

            _userCommands = new Dictionary<string, GoAsDelegate>()
                            {
                                { UserCommands.User, (args, loadText, loadWords) => CommandExecutor(args, loadText, GoAsUser)},
                                { UserCommands.Administrator, (args, loadText, loadWords) => CommandExecutor(args, loadWords, GoAsAdmin)},
                                { UserCommands.Reader, (args, loadText, loadWords) => CommandExecutor(args, loadText, GoAsReader)},
                                { UserCommands.ContentCurator, (args, loadText, loadWords) => CommandExecutor(args, loadText, GoAsContentCurator)},
                            };
        }

        private static ICommandResult CommandExecutor(string[] args, LoadFunctionDelegate load, Func<string, ICommandResult> goWithRole)
        {
            var loadResult = load(args);

            if (!loadResult.OK)
                return loadResult;

            return goWithRole((loadResult as CommandResultWithText).Text);
        }

        public ICommandResult ProcessCommand(string[] args, LoadFunctionDelegate loadText, LoadFunctionDelegate loadWords)
        {
            if (args.Length < 1)
                return new CommandResult(false, "No command was passed");

            if (!_userCommands.TryGetValue(args[0], out GoAsDelegate goWithRole))
                return new CommandResult(false, "Unknown command");

            return goWithRole(args, loadText, loadWords);
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
                builder.Append(string.Join(" ", words));

                return new CommandResult(true, builder.ToString());
            }

            return new CommandResult(false, result.Message);
        }

        private ICommandResult GoAsUser(string text)
        {
            var result = _controller.ScanTextForWords(text);

            return new CommandResult(true, BuildOutput("Scanned the text:", text, result.Message));
        }

        private ICommandResult GoAsReader(string text)
        {
            if (!_controller.TryHideWords(text, out string resultText))
            {
                return new CommandResult(false, "Text was empty");
            }

            return new CommandResultWithText(resultText);
        }

        private ICommandResult GoAsContentCurator(string text)
        {
            var result = _controller.ScanTextForWords(text);

            return new CommandResult(true, BuildOutput(result.Message, "Unfiltered text:", text));
        }

        private string BuildOutput(params string[] lines)
        {
            if (lines == null)
                return string.Empty;
            var builder = new StringBuilder();
            for (var i = 0; i < lines.Length; i++)
            {
                if (i == lines.Length - 1) {
                    builder.Append(lines[i]);
                }
                else {
                    builder.AppendLine(lines[i]);
                }
            }

            return builder.ToString();
        }
    }
}

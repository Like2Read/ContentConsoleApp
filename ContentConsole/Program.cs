using ContentConsole.CommandResults;
using DataControl;
using DataControl.Mock;
using System;
using System.IO;
using System.Linq;
using System.Configuration;

namespace ContentConsole
{
    public static class Program
    {
        private static string _dictionaryFile;
        private static string _path;
        private static string _textFile;
        private static IDataControllerFactory _factory;
        private static CommandProcessor _commandProcessor;

        private static Func<string, string> _loadWords = (path) => string.Join(" ", File.ReadAllLines(path).Where(x => x.Trim().Length > 0));

        public static void Main(string[] args)
        {
            if (!CheckArguments(args))
                return;

            _path = ConfigurationManager.AppSettings["file-path"].ToString();
            _dictionaryFile = ConfigurationManager.AppSettings["dictionary-file"].ToString();
            _textFile = ConfigurationManager.AppSettings["text-file"].ToString();

            if (!Initialize(_path, _dictionaryFile))
                return;

            var result = _commandProcessor.ProcessCommand(args,
                   arguments => InputTextController.ReadTextFromFile(arguments, File.ReadAllText),
                   arguments => InputTextController.ReadTextFromFile(arguments, _loadWords)
                );

            result.Print(Console.Out);
            Console.Write("Press ANY key to exit.");
            Console.ReadKey();

        }

        private static bool Initialize(string path, string dictionaryFile)
        {
            if (path == null)
            {
                Console.WriteLine($"Folder was not set up");
                return false;
            }

            if (path == null)
            {
                Console.WriteLine($"File with dictionary was not set up");
                return false;
            }

            if (!InputTextController.TryReadWords(Path.Combine(path, dictionaryFile), out string[] words))
            {
                Console.WriteLine($"Dictionary {dictionaryFile} was not found in Files folder!");
                return false;
            }

            _factory = new DataControllerFactory();
            _commandProcessor = new CommandProcessor(_factory.Create(words));

            return true;
        }

        private static bool CheckArguments(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(@"Application should be called with commands: 
u - user mode: ContentConsole.exe u original.txt
a - administrator mode: ContentConsole.exe a new-dictionary.txt
r - reader mode: ContentConsole.exe r original.txt
c - content curator mode: ContentConsole.exe c original.txt
");
                return false;
            }

            return true;
        }


    }

}

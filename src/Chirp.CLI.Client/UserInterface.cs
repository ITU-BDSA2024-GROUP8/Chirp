using SimpleDB;
using DocoptNet;

namespace Chirp.CLI.Client
{
    public static class UserInterface
    {
        private const string usage = @"

Usage:
  chirp read <limit>
  chirp readAll
  chirp cheep <message>
  chirp (h | help)
  chirp version

Options:
  h/help     Show this screen.
  version     Show version.
";

        public static void Run(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
            CSVDatabase<App.Cheep> db = new CSVDatabase<App.Cheep>();

            HandleArguments(arguments, db);
        }

        private static void HandleArguments(IDictionary<string, ValueObject> arguments, CSVDatabase<App.Cheep> db)
        {
            if (arguments["read"].IsTrue)
            {
                var limit = int.Parse(arguments["<limit>"].ToString()); 
                IEnumerable<App.Cheep> cheeps = db.Read(limit); 
                PrintCheeps(cheeps);
            }
            else if (arguments["readAll"].IsTrue)
            {
                IEnumerable<App.Cheep> cheeps = db.Read();
                PrintCheeps(cheeps);
            }
            else if (arguments["cheep"].IsTrue)
            {
                var message = arguments["<message>"].ToString();
                App.Cheep cheep = new App.Cheep(Environment.UserName, $"\"{message}\"", DateTimeOffset.Now.ToUnixTimeSeconds());
                db.Store(cheep);
                CheepStoredMsg();
            }
            else if (arguments["help"].IsTrue || arguments["h"].IsTrue)
            {
                Console.WriteLine(usage);
            }
            else if (arguments["version"].IsTrue)
            {
                Console.WriteLine("Chirp CLI version 1.0");
            }
        }

        private static void PrintCheeps(IEnumerable<App.Cheep> cheeps)
        {
            foreach (var cheep in cheeps)
            {
                var timestamp = Util.FromSecondsToDateAndTime(cheep.Timestamp);
                Console.WriteLine($"{cheep.Author} @ {timestamp}: {cheep.Message}");
            }
        }

        private static void CheepStoredMsg()
        {
            Console.WriteLine("Cheep stored successfully!");
        }
    }
}
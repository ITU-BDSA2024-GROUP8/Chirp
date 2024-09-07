// Chirp.CLI.Client/Program.cs
using SimpleDB;
using DocoptNet;

namespace Chirp.CLI.Client
{
    public class App
    {
        private const string usage = @"Chirp CLI version.

Usage:
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

        static void Main(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
            CSVDatabase<Cheep> db = new CSVDatabase<Cheep>();

            if (arguments["read"].IsTrue)
            {
                int limit = arguments["<limit>"] != null ? int.Parse(arguments["<limit>"].ToString()) : 0;
                IEnumerable<Cheep> cheeps = db.Read(limit);
                UserInterface.PrintCheeps(cheeps);
            }
            else if (arguments["cheep"].IsTrue)
            {
                string message = arguments["<message>"].ToString();
                Cheep cheep = new Cheep(Environment.UserName, $"\"{message}\"", DateTimeOffset.Now.ToUnixTimeSeconds());
                db.Store(cheep);
                UserInterface.CheepStoredMSG();
            }
            else if (arguments["--help"].IsTrue)
            {
                Console.WriteLine(usage);
            }
            else if (arguments["--version"].IsTrue)
            {
                Console.WriteLine("Chirp CLI version 1.0");
            }
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
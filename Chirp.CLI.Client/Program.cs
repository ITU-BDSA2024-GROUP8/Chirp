using SimpleDB;

// Use dotnet run -- read to run the code
namespace Chirp.CLI.Client
{
    class App
    {
        static void Main(string[] args)
        {
            CSVDatabase<Cheep> db = new CSVDatabase<Cheep>();
            
            if (args.Length > 0 && args[0] == "read")
            {
                IEnumerable<Cheep> cheeps = db.Read();
                PrintCheeps(cheeps);
            } else if (args.Length > 0 && args[0] == "cheep" && args.Length == 2)
            {
                Cheep cheep = new Cheep(Environment.UserName, $"\"{args[1]}\"", DateTimeOffset.Now.ToUnixTimeSeconds());
                db.Store(cheep);
            }
        }

        private static void PrintCheeps(IEnumerable<Cheep> cheeps)
        {
            foreach (var cheep in cheeps)
            {
                string timestamp = Util.FromSecondsToDateAndTime(cheep.Timestamp);
                Console.WriteLine($"{cheep.Author} @ {timestamp}: {cheep.Message}");
            }
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
using SimpleDB;

// Use dotnet run -- read to run the code
namespace Chirp.CLI.Client
{
    class App
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "read")
            {
                CSVDatabase<Cheep> db = new CSVDatabase<Cheep>();
                IEnumerable<Cheep> cheeps = db.Read();
                
                PrintCheeps(cheeps);
            }
        }

        private static void PrintCheeps(IEnumerable<Cheep> cheeps)
        {
            foreach (var cheep in cheeps)
            {
                string timestamp = Util.FromUnixTimeMilliseconds(cheep.Timestamp).ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"{cheep.Author} @ {timestamp}: {cheep.Message}");
            }
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
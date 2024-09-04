using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

// Use dotnet run -- read to run the code
namespace Chirp.CLI.Client
{
    class App
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "read")
            {
                using var reader = new StreamReader("../SimpleDB/Data/Data.csv");
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                });

                var cheeps = csv.GetRecords<Cheep>();

                foreach (var cheep in cheeps)
                {
                    string timestamp = FromUnixTimeMilliseconds(cheep.Timestamp).ToString("yyyy-MM-dd HH:mm:ss");
                    Console.WriteLine($"{cheep.Author} @ {timestamp}: {cheep.Message}");
                }
            }
        }

        private static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
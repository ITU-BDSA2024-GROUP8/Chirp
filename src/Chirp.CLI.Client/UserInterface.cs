using DocoptNet;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Chirp.CSVDBService;

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

        public static async Task Run(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
            CSVDatabase<App.Cheep> db = CSVDatabase<App.Cheep>.Instance;

            await HandleArguments(arguments, db);
        }

        private static async Task HandleArguments(IDictionary<string, ValueObject> arguments, CSVDatabase<App.Cheep> db)
        {
            var baseURL = "http://localhost:5279";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseURL);
            
            if (arguments["read"].IsTrue)
            {
                try
                {
                    var limit = int.Parse(arguments["<limit>"].ToString()); 
                    IEnumerable<App.Cheep> cheeps = await client.GetFromJsonAsync<IEnumerable<App.Cheep>>($"cheep/{limit}") ??
                                 Enumerable.Empty<App.Cheep>(); 
                    PrintCheeps(cheeps);
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            else if (arguments["readAll"].IsTrue)
            {
                try
                {
                    var cheeps = await client.GetFromJsonAsync<IEnumerable<App.Cheep>>("cheeps") ??
                                 Enumerable.Empty<App.Cheep>();
                    PrintCheeps(cheeps);
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            else if (arguments["cheep"].IsTrue)
            {
                try
                {
                    var message = arguments["<message>"].ToString();
                    App.Cheep cheep = new App.Cheep(Environment.UserName, $"\"{message}\"", DateTimeOffset.Now.ToUnixTimeSeconds());
                    await client.PostAsJsonAsync("cheep", cheep);
                    CheepStoredMsg();
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
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
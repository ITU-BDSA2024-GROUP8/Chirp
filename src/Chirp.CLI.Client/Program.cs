using SimpleDB;
using DocoptNet;

namespace Chirp.CLI.Client
{
    public class App
    {
        static async Task Main(string[] args)
        {
            await UserInterface.Run(args);
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
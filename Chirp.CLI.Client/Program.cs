using SimpleDB;
using DocoptNet;

namespace Chirp.CLI.Client
{
    public class App
    {
        static void Main(string[] args)
        {
            UserInterface.Run(args);
        }

        public record Cheep(string Author, string Message, long Timestamp);
    }
}
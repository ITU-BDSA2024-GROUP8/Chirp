// Chirp.CLI.Client/UserInterface.cs
namespace Chirp.CLI.Client
{
    public static class UserInterface
    {
        public static void PrintCheeps(IEnumerable<App.Cheep> cheeps)
        {
            foreach (var cheep in cheeps)
            {
                string timestamp = Util.FromSecondsToDateAndTime(cheep.Timestamp);
                Console.WriteLine($"{cheep.Author} @ {timestamp}: {cheep.Message}");
            }
        }

        public static void CheepStoredMSG()
        {
            Console.WriteLine("Cheep stored successfully!");
        }
    }
}
/*
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SimpleDB; 
using Chirp.CLI.Client;

namespace Chirp.test.UserInterfaceTests

{
public class UserInterfaceTests {
  private record Cheep(string Author, string Message, long Timestamp);
    [Fact]
    public void testReadStore() {
        Cheep cheep = new Cheep("adot", "Hey there! new test", 2);
        CSVDatabase<Cheep> csvDatabase = CSVDatabase<Cheep>.Instance;
        csvDatabase.Store(cheep);

        IEnumerable<Cheep> cheeps = new List<Cheep> { cheep };
     

     using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                UserInterface.PrintCheeps(cheeps);

                // Assert
                var expectedOutput = "adot: Hey there! at 2";
                                   
                Assert.Equal(expectedOutput, sw.ToString());
            }
      //clean up
      ClearCSV();

    }


    private void ClearCSV()
        {
            var lines = File.ReadAllLines("../../../../data/database.csv").ToList();
            if (lines.Count > 0)
            {
                lines.RemoveAt(lines.Count - 1);
                File.WriteAllLines("../../../../data/database.csv", lines);
            }
        }
}

}
*/
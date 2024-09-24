using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using SimpleDB;

namespace Chirp.test.IntegrationTest
{
public class IntegrationTest {
  private record Cheep(string Author, string Message, long Timestamp);
    [Fact]
    public void testReadStore() {
  
        CSVDatabase<Cheep> csvDatabase = CSVDatabase<Cheep>.Instance;
        Cheep cheep = new Cheep("adot", "Hey there! new test", 2);

      
        csvDatabase.Store(cheep);
        var output = csvDatabase.Read().Last();

        
        Assert.Equal(cheep.Author, output.Author);
        Assert.Equal(cheep.Message, output.Message);
        Assert.Equal(cheep.Timestamp, output.Timestamp);

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
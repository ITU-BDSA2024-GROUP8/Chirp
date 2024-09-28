
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Chirp.CLI.Client;
using SimpleDB;

namespace Chirp.test.IntegrationTest
{
  public class IntegrationTest
  {

    [Fact]
    public void testReadStore()
    {


      CSVDatabase<App.Cheep> csvDatabase = CSVDatabase<App.Cheep>.Instance;
      App.Cheep cheep = new App.Cheep("adot", "Hey there! new test", 2);
      //clean up
      ClearCSV();
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
      var header = "Author,Message,Timestamp";
      File.WriteAllLines("../../../../data/databaseForTesting.csv", new List<string> { header });
    }
  }
}

/*
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SimpleDB; 
using Chirp.CLI.Client;
using DocoptNet;


namespace Chirp.test.UserInterfaceTests

{
public class UserInterfaceTests {

    [Fact]
    public void testReadStore() {
        //clean up
        

        App.Cheep cheep = new App.Cheep("adot", "Hey there! new test", 2);
        CSVDatabase<App.Cheep> csvDatabase = CSVDatabase<App.Cheep>.Instance;
        ClearCSV();
        csvDatabase.Store(cheep);

      IDictionary<string, ValueObject> arguments = new Docopt().Apply(UserInterface.usage, new string[] {"read", "1"}, version: "1.0", exit: true)!;

     using (var sw = new StringWriter())
            {
         Console.SetOut(sw);
                // Act
                UserInterface.HandleArguments(arguments, csvDatabase);
               
                // Assert
                 var timestamp = Util.FromSecondsToDateAndTime(cheep.Timestamp);
                var expectedOutput = ($"{cheep.Author} @ {timestamp}: {cheep.Message}");;
                var Output = sw.ToString().Trim();                   
                Assert.Equal(expectedOutput, Output);
               
        
                 
            }
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
*/
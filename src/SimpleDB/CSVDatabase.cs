using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? instance = null;
    private string filePath = "../../data/database.csv";

    private CSVDatabase()
    {
        filePath = DetermineFilePath();
    }

   private string DetermineFilePath()
        {
            string[] possiblePaths = {
                "../../data/database.csv",
                "../../../../data/database.csv"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Default to the first path if none exist
            return possiblePaths[0];
        }
   
    
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CSVDatabase<T>();
            }
            return instance;
        }
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        IEnumerable<T> cheeps = csv.GetRecords<T>().ToList();
        
        return limit.HasValue ? cheeps.Take(limit.Value) : cheeps;
    }

    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ShouldQuote = (_) => false,
        };
        
        using (var writer = new StreamWriter(filePath, append: true))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecord(record);
            writer.WriteLine();
        }
    }
}
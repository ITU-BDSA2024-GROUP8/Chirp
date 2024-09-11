using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string filePath;

    public CSVDatabase()
    {
        filePath = "../../data/database.csv";
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        IEnumerable<T> cheeps = csv.GetRecords<T>().ToList();
        
        return limit.HasValue ? cheeps.Take(limit.Value) : cheeps;;
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
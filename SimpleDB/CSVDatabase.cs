using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/Data/Data.csv");
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        IEnumerable<T> cheeps = csv.GetRecords<T>().ToList();
        
        return cheeps;
    }

    public void Store(T record)
    {
        throw new Exception();
    }
}
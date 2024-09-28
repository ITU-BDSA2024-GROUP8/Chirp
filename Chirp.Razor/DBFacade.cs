using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade : IDBFacade
{
    private readonly string _dbPath;
    
    public DBFacade(string dbPath)
    {
        _dbPath = dbPath;
    }
    
    public void createDB()
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = embeddedProvider.GetFileInfo("data/schema.sql").CreateReadStream();
        using var sr = new StreamReader(reader);
        
        var query = sr.ReadToEnd();
        
        // Creates database file and executes SQL query from data/schema.sql file.
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
        
            var command = connection.CreateCommand();
            command.CommandText = query;
        
            command.ExecuteNonQuery();
        }
    }
    
    public bool dbExists(string path)
    {
        return File.Exists(path);
    }

    public void addDummyData()
    {
        
    }
}
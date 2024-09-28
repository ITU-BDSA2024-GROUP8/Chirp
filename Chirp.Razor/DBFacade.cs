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
        Console.WriteLine("Creating DB...");
        string query = getQueryFromEmbeddedFile("data/schema.sql");
        
        // Creates database file and executes SQL query from data/schema.sql file.
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
        
            var command = connection.CreateCommand();
            command.CommandText = query;
        
            command.ExecuteNonQuery();
            addDummyData();
        }
    }
    
    public bool dbExists(string path)
    {
        return File.Exists(path);
    }

    public void addDummyData(SqliteConnection? connection = null)
    {
        string query = getQueryFromEmbeddedFile("data/dump.sql");
        
        if (connection is null)
        {
            using (var dbconnection = new SqliteConnection($"Data Source={_dbPath}"))
            {
                dbconnection.Open();
        
                var command = dbconnection.CreateCommand();
                command.CommandText = query;
        
                command.ExecuteNonQuery();
            }
        }
        else
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
        
            command.ExecuteNonQuery();
        }
    }

    public string getQueryFromEmbeddedFile(string filePath)
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = embeddedProvider.GetFileInfo(filePath).CreateReadStream();
        using var sr = new StreamReader(reader);
        
        return sr.ReadToEnd();
    }
}
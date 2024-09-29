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

    public List<CheepViewModel> getCheeps()
    {
        var query = @"
            SELECT * FROM message M
            JOIN user U 
            ON M.author_id = U.user_id
            ORDER by M.pub_date desc
            LIMIT 32
        ";

        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
        
            var command = connection.CreateCommand();
            command.CommandText = query;
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var message = reader.GetString(2);
                    var author = reader.GetString(5);
                    var pubDate = CheepService.UnixTimeStampToDateTimeString(Double.Parse(reader.GetString(3) as string));

                    cheeps.Add(new CheepViewModel(author, message, pubDate)); 
                }
            }
        }

        return cheeps;
    }
}
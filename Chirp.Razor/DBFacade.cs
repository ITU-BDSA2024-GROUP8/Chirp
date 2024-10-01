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

    public List<CheepViewModel> getCheeps(int page)
    {
        var query = @"
            SELECT M.text, M.pub_date, U.username FROM message M
            JOIN user U 
            ON M.author_id = U.user_id
            ORDER by M.pub_date desc
            LIMIT 32
            OFFSET @offset
        ";

        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
        
            var command = connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@offset", (page*32)-32);
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var message = reader.GetString(0);
                    var author = reader.GetString(2);
                    var pubDate = CheepService.UnixTimeStampToDateTimeString(Double.Parse(reader.GetString(1) as string));

                    cheeps.Add(new CheepViewModel(author, message, pubDate)); 
                }
            }
        }

        return cheeps;
    }

        public List<CheepViewModel> getCheepsFromAuthor(int page, string author)
    {
        // filter by the provided author name
        var query = @"
            SELECT M.text, M.pub_date FROM message M
            JOIN user U 
            ON M.author_id = U.user_id
            WHERE U.username = @author
            ORDER by M.pub_date desc
            LIMIT 32
            OFFSET @offset
        ";

        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            connection.Open();
        
            var command = connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@offset", (page*32)-32);
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var message = reader.GetString(0);
                    var pubDate = CheepService.UnixTimeStampToDateTimeString(Double.Parse(reader.GetString(1) as string));

                    cheeps.Add(new CheepViewModel(author, message, pubDate)); 
                }
            }
        }

        return cheeps;
    }
}
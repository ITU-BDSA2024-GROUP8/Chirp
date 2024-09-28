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
        Console.WriteLine("Created Database");
    }
    
    public bool dbExists(string path)
    {
        return File.Exists(path);
    }

    public void addDummyData()
    {
        
    }
}
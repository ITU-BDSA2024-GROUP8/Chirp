namespace Chirp.Razor;

public class DBFacade
{
    public void createDB()
    {
        
    }

    public string getDbPath()
    {
        return Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
    }
    
    public bool dbExists(string path)
    {
        return false;
    }

    public void addDummyData()
    {
        
    }
}
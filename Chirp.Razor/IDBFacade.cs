namespace Chirp.Razor;

public interface IDBFacade
{
    public void createDB();

    public bool dbExists(string path);

    public void addDummyData();
}
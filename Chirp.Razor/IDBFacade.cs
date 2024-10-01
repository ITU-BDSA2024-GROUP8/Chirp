namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

public interface IDBFacade
{
    public void createDB();

    public bool dbExists(string path);

    public void addDummyData(SqliteConnection? connection);

    public List<CheepViewModel> getCheeps(int page);

    public List<CheepViewModel> getCheepsFromAuthor(int page, string author);
}
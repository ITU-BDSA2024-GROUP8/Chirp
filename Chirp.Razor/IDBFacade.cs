namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

public interface IDBFacade
{
    public void createDB();

    public bool dbExists(string path);

    public void addDummyData(SqliteConnection? connection);

    public List<CheepViewModel> getCheeps();

    public List<CheepViewModel> getCheepsFromAuthor(string author);
}
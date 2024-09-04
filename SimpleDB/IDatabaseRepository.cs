namespace SimpleDB;

interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null);
    public void Store(T record);
    public record Cheep(string Author, string Message, long Timestamp);
}
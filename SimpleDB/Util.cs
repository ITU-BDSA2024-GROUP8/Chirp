namespace SimpleDB;

public class Util
{
    public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }
}
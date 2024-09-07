using System.Globalization;

namespace SimpleDB;

public static class Util
{
    public static string FromSecondsToDateAndTime(long seconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
        DateTime dateTime = dateTimeOffset.UtcDateTime;
        return dateTime.ToLocalTime().ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
namespace VecoBackend.Utils;

public class Converter
{
    public static long ToUnixTime(DateTime date)
    {
        return (long)(date - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
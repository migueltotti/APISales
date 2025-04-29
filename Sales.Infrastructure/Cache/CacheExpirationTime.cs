namespace Sales.Infrastructure.Cache;

public class CacheExpirationTime
{
    private static TimeSpan _oneDay = TimeSpan.FromDays(1);
    private static TimeSpan _oneWeek = TimeSpan.FromDays(7);
    private static TimeSpan _oneMonth = TimeSpan.FromDays(30);
    private static TimeSpan _fiveHours = TimeSpan.FromHours(5);
    private static TimeSpan _sevenHours = TimeSpan.FromHours(7);

    public static TimeSpan OneDay => _oneDay;
    
    public static TimeSpan OneWeek => _oneWeek;
    
    public static TimeSpan OneMonth => _oneMonth;
    
    public static TimeSpan FiveHours => _fiveHours;
    
    public static TimeSpan SevenHours => _sevenHours;
}
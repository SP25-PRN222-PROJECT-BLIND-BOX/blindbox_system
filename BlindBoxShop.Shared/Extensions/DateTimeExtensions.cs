namespace BlindBoxShop.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetCurrentTimeInTimeZone(this DateTime datetime, string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime utcTime = datetime.Kind == DateTimeKind.Utc ? datetime : datetime.ToUniversalTime();
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        }

        public static DateTime SEAsiaStandardTime(this DateTime datetime)
        {
            return datetime.GetCurrentTimeInTimeZone("SE Asia Standard Time");
        }
    }
}

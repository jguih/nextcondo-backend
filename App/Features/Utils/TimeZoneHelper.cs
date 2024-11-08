namespace NextCondoApi.Utils;

public static class TimeZoneHelper
{
    public static TimeOnly ConvertFromUserTimeToUTC(TimeOnly userTime, int timezoneOffsetMinutes)
    {
        return userTime.AddMinutes(-timezoneOffsetMinutes);
    }

    public static TimeOnly ConvertFromUTCToUserTime(TimeOnly utcTime, int timezoneOffsetMinutes)
    {
        return utcTime.AddMinutes(timezoneOffsetMinutes);
    }

    public static DateTime GetUserDateTime(int timezoneOffsetMinutes)
    {
        return DateTime.UtcNow.AddMinutes(timezoneOffsetMinutes);
    }

    /// <summary>
    /// Returns if startAt is earlier than endAt, considering midnight (00:00) as the end of day (24:00).
    /// </summary>
    /// <param name="startAt"></param>
    /// <param name="endAt"></param>
    /// <returns></returns>
    public static bool IsEarlierThan(TimeOnly startAt, TimeOnly endAt)
    {
        // Check if the end time is midnight (00:00) and treat it as the end of the day (24:00)
        if (endAt == TimeOnly.MinValue)
        {
            return true;
        }

        return startAt < endAt;
    }
}
public static class WorkingCalendar
{
    // Weekday closures for 2024: Danish public holidays, Grundlovsdag,
    // Christmas Eve, and New Year's Eve. Store Bededag was abolished in 2024.
    private static readonly HashSet<DateOnly> Holidays =
    [
        new(2024, 1, 1),
        new(2024, 3, 28),
        new(2024, 3, 29),
        new(2024, 3, 31),
        new(2024, 4, 1),
        new(2024, 5, 9),
        new(2024, 5, 19),
        new(2024, 5, 20),
        new(2024, 6, 5),
        new(2024, 12, 24),
        new(2024, 12, 25),
        new(2024, 12, 26),
        new(2024, 12, 31),
    ];

    public static IReadOnlyList<DateOnly> WorkingDays(int year, int month)
    {
        if (year != 2024)
            throw new ArgumentOutOfRangeException(nameof(year), "Only 2024 is supported.");
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        var days = new List<DateOnly>();
        var date = new DateOnly(year, month, 1);
        var nextMonth = date.AddMonths(1);
        for (; date < nextMonth; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;
            if (Holidays.Contains(date))
                continue;
            days.Add(date);
        }

        return days;
    }
}

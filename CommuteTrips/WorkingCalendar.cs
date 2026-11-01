public static class WorkingCalendar
{
    // Danish public holidays, Grundlovsdag, Christmas Eve, and New Year's Eve.
    // Store Bededag was still a public holiday in 2023 and was abolished in 2024.
    private static readonly HashSet<DateOnly> Holidays =
    [
        new(2023, 1, 1),
        new(2023, 4, 6),
        new(2023, 4, 7),
        new(2023, 4, 9),
        new(2023, 4, 10),
        new(2023, 5, 5),
        new(2023, 5, 18),
        new(2023, 5, 28),
        new(2023, 5, 29),
        new(2023, 6, 5),
        new(2023, 12, 24),
        new(2023, 12, 25),
        new(2023, 12, 26),
        new(2023, 12, 31),
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
        new(2025, 1, 1),
        new(2025, 4, 17),
        new(2025, 4, 18),
        new(2025, 4, 20),
        new(2025, 4, 21),
        new(2025, 5, 29),
        new(2025, 6, 5),
        new(2025, 6, 8),
        new(2025, 6, 9),
        new(2025, 12, 24),
        new(2025, 12, 25),
        new(2025, 12, 26),
        new(2025, 12, 31),
    ];

    public static IReadOnlyList<DateOnly> WorkingDays(int year, int month)
    {
        if (year is not (2023 or 2024 or 2025))
            throw new ArgumentOutOfRangeException(nameof(year), "Only 2023, 2024, and 2025 are supported.");
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

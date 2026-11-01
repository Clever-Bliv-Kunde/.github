public class WorkingCalendarTests
{
    [Theory]
    [InlineData(1, 22)]
    [InlineData(2, 21)]
    [InlineData(3, 19)]
    [InlineData(4, 21)]
    [InlineData(5, 21)]
    [InlineData(6, 19)]
    [InlineData(7, 23)]
    [InlineData(8, 22)]
    [InlineData(9, 21)]
    [InlineData(10, 23)]
    [InlineData(11, 21)]
    [InlineData(12, 18)]
    public void Each_month_of_2024_excludes_weekends_and_holidays(int month, int expected) =>
        Assert.Equal(expected, WorkingCalendar.WorkingDays(2024, month).Count);

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 28)]
    [InlineData(3, 29)]
    [InlineData(3, 31)]
    [InlineData(4, 1)]
    [InlineData(5, 9)]
    [InlineData(5, 19)]
    [InlineData(5, 20)]
    [InlineData(6, 5)]
    [InlineData(12, 24)]
    [InlineData(12, 25)]
    [InlineData(12, 26)]
    [InlineData(12, 31)]
    public void Danish_holidays_are_not_working_days(int month, int day)
    {
        var days = WorkingCalendar.WorkingDays(2024, month);
        Assert.DoesNotContain(new DateOnly(2024, month, day), days);
    }

    [Fact]
    public void Ordinary_weekdays_and_leap_day_are_working_days()
    {
        Assert.Contains(new DateOnly(2024, 1, 2), WorkingCalendar.WorkingDays(2024, 1));
        Assert.Contains(new DateOnly(2024, 2, 29), WorkingCalendar.WorkingDays(2024, 2));
        Assert.Contains(new DateOnly(2024, 12, 23), WorkingCalendar.WorkingDays(2024, 12));
        Assert.Contains(new DateOnly(2024, 12, 30), WorkingCalendar.WorkingDays(2024, 12));
    }

    [Theory]
    [InlineData(1, 22)]
    [InlineData(2, 20)]
    [InlineData(3, 21)]
    public void Early_2025_excludes_weekends_and_new_years_day(int month, int expected) =>
        Assert.Equal(expected, WorkingCalendar.WorkingDays(2025, month).Count);

    [Theory]
    [InlineData(5, 20)]
    [InlineData(6, 21)]
    [InlineData(7, 21)]
    [InlineData(8, 23)]
    [InlineData(9, 21)]
    [InlineData(10, 22)]
    [InlineData(11, 22)]
    [InlineData(12, 19)]
    public void May_through_december_2023_excludes_weekends_and_holidays(int month, int expected) =>
        Assert.Equal(expected, WorkingCalendar.WorkingDays(2023, month).Count);

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, 18)]
    [InlineData(5, 29)]
    [InlineData(6, 5)]
    [InlineData(12, 25)]
    [InlineData(12, 26)]
    public void The_2023_holidays_in_the_commute_range_are_closed(int month, int day) =>
        Assert.DoesNotContain(new DateOnly(2023, month, day), WorkingCalendar.WorkingDays(2023, month));

    [Fact]
    public void May_first_2023_is_a_commute_day() =>
        Assert.Contains(new DateOnly(2023, 5, 1), WorkingCalendar.WorkingDays(2023, 5));

    [Fact]
    public void New_years_day_2025_is_not_a_working_day()
    {
        var january = WorkingCalendar.WorkingDays(2025, 1);
        Assert.DoesNotContain(new DateOnly(2025, 1, 1), january);
        Assert.Contains(new DateOnly(2025, 1, 2), january);
    }

    [Fact]
    public void Weekends_are_not_working_days()
    {
        var january = WorkingCalendar.WorkingDays(2024, 1);
        Assert.DoesNotContain(new DateOnly(2024, 1, 6), january);
        Assert.DoesNotContain(new DateOnly(2024, 1, 7), january);
        Assert.All(january, day =>
            Assert.True(day.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday));
    }
}

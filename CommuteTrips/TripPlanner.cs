public sealed record Ride(
    string FromStreet,
    string ToStreet,
    DateTime Departure,
    string Driver,
    int Seats,
    string State,
    int PriceDkk,
    string FromCity,
    string ToCity);

public static class TripPlanner
{
    public static IReadOnlyList<Ride> Plan(int year, IReadOnlyList<int> roundTripsByMonth, Random random)
    {
        if (roundTripsByMonth.Count != 12)
            throw new ArgumentException("Expected one count for each month.", nameof(roundTripsByMonth));

        var rides = new List<Ride>();
        for (var month = 1; month <= 12; month++)
        {
            var count = roundTripsByMonth[month - 1];
            var days = WorkingCalendar.WorkingDays(year, month);
            if (count < 0 || count > days.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(roundTripsByMonth),
                    count,
                    $"Month {month} allows at most {days.Count} round trips.");
            }

            foreach (var day in Spread(days, count))
                rides.AddRange(RoundTrip(day, random));
        }

        return rides;
    }

    private static IEnumerable<Ride> RoundTrip(DateOnly day, Random random)
    {
        var origin = AddressCatalog.KoldingArea[random.Next(AddressCatalog.KoldingArea.Count)];
        var destination = AddressCatalog.CopenhagenArea[random.Next(AddressCatalog.CopenhagenArea.Count)];
        var morning = day.ToDateTime(Slot(random, 6));
        var morningDriver = Drivers.Names[random.Next(Drivers.Names.Count)];
        var morningPrice = Price(random);
        var evening = day.ToDateTime(Slot(random, 17));
        var eveningDriver = Drivers.Names[random.Next(Drivers.Names.Count)];
        var eveningPrice = Price(random);

        yield return new Ride(
            origin.Street,
            destination.Street,
            morning,
            morningDriver,
            1,
            "Completed",
            morningPrice,
            origin.City,
            "København");
        yield return new Ride(
            destination.Street,
            origin.Street,
            evening,
            eveningDriver,
            1,
            "Completed",
            eveningPrice,
            "København",
            origin.City);
    }

    private static IEnumerable<DateOnly> Spread(IReadOnlyList<DateOnly> days, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var index = ((2 * i) + 1) * days.Count / (2 * count);
            yield return days[index];
        }
    }

    private static TimeOnly Slot(Random random, int startHour)
    {
        var minutes = startHour * 60 + random.Next(16) * 15;
        return new TimeOnly(minutes / 60, minutes % 60);
    }

    private static int Price(Random random) => 140 + 5 * random.Next(25);
}

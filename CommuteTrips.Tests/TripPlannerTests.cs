public class TripPlannerTests
{
    [Fact]
    public void A_round_trip_is_two_rides_on_the_same_working_day()
    {
        var rides = TripPlanner.Plan(2024, Counts(1, 1), new Random(1));

        Assert.Equal(2, rides.Count);
        Assert.Equal(rides[0].Departure.Date, rides[1].Departure.Date);
        Assert.Equal(new DateOnly(2024, 1, 17), DateOnly.FromDateTime(rides[0].Departure));
        Assert.True(rides[0].Departure.TimeOfDay > rides[1].Departure.TimeOfDay);
    }

    [Fact]
    public void Requested_days_are_spread_across_the_month()
    {
        var rides = TripPlanner.Plan(2024, Counts(8, 4), new Random(3));
        var days = rides
            .Where(ride => ride.Departure.Hour < 12)
            .Select(ride => DateOnly.FromDateTime(ride.Departure))
            .OrderBy(day => day)
            .ToArray();

        Assert.Equal(
            [
                new DateOnly(2024, 8, 5),
                new DateOnly(2024, 8, 13),
                new DateOnly(2024, 8, 20),
                new DateOnly(2024, 8, 28)
            ],
            days);
    }

    [Fact]
    public void One_round_trip_uses_every_working_day_when_the_month_is_full()
    {
        var counts = Enumerable.Range(1, 12)
            .Select(month => WorkingCalendar.WorkingDays(2024, month).Count)
            .ToArray();
        var rides = TripPlanner.Plan(2024, counts, new Random(4));
        var mornings = rides.Where(ride => ride.Departure.Hour < 12).Select(ride => DateOnly.FromDateTime(ride.Departure)).ToHashSet();
        var expected = Enumerable.Range(1, 12).SelectMany(month => WorkingCalendar.WorkingDays(2024, month)).ToHashSet();

        Assert.Equal(expected.OrderBy(day => day), mornings.OrderBy(day => day));
        Assert.Equal(expected.Count * 2, rides.Count);
    }

    [Fact]
    public void Outbound_is_a_morning_ride_toward_copenhagen_and_return_is_the_same_evening()
    {
        var rides = TripPlanner.Plan(2024, Counts(3, 2), new Random(8));
        var west = AddressCatalog.KoldingArea.Select(place => place.Street).ToHashSet();
        var east = AddressCatalog.CopenhagenArea.Select(place => place.Street).ToHashSet();
        var westCities = AddressCatalog.KoldingArea.Select(place => place.City).ToHashSet();

        Assert.Equal(4, rides.Count);
        foreach (var day in rides.GroupBy(ride => ride.Departure.Date))
        {
            var outbound = day.Single(ride => ride.Departure.Hour < 12);
            var inbound = day.Single(ride => ride.Departure.Hour >= 12);

            Assert.Contains(outbound.FromStreet, west);
            Assert.Contains(outbound.ToStreet, east);
            Assert.Contains(outbound.FromCity, westCities);
            Assert.Equal("København", outbound.ToCity);
            Assert.Equal(outbound.ToStreet, inbound.FromStreet);
            Assert.Equal(outbound.FromStreet, inbound.ToStreet);
            Assert.Equal("København", inbound.FromCity);
            Assert.Equal(outbound.FromCity, inbound.ToCity);
            Assert.Equal(outbound.Departure.Date, inbound.Departure.Date);
            AssertMorning(outbound.Departure);
            AssertEvening(inbound.Departure);
        }
    }

    [Fact]
    public void Every_ride_is_one_completed_seat_with_a_price_in_steps_of_five()
    {
        var counts = Enumerable.Repeat(3, 12).ToArray();
        var rides = TripPlanner.Plan(2024, counts, new Random(11));

        Assert.All(rides, ride =>
        {
            Assert.Equal(1, ride.Seats);
            Assert.Equal("Completed", ride.State);
            Assert.InRange(ride.PriceDkk, 140, 260);
            Assert.Equal(0, (ride.PriceDkk - 140) % 5);
            Assert.Contains(ride.Driver, Drivers.Names);
        });
    }

    [Fact]
    public void Each_leg_draws_its_own_driver()
    {
        var foundDifferentDrivers = Enumerable.Range(1, 30).Any(seed =>
        {
            var rides = TripPlanner.Plan(2024, Counts(1, 1), new Random(seed));
            return rides[0].Driver != rides[1].Driver;
        });

        Assert.True(foundDifferentDrivers);
    }

    [Fact]
    public void The_same_seed_reproduces_the_same_rides()
    {
        var counts = Counts(6, 5);
        var first = RideFormatter.Format(TripPlanner.Plan(2024, counts, new Random(42)));
        var second = RideFormatter.Format(TripPlanner.Plan(2024, counts, new Random(42)));
        var other = RideFormatter.Format(TripPlanner.Plan(2024, counts, new Random(43)));

        Assert.Equal(first, second);
        Assert.NotEqual(first, other);
    }

    [Fact]
    public void Rides_are_printed_newest_first()
    {
        var rides = TripPlanner.Plan(2024, Enumerable.Repeat(2, 12).ToArray(), new Random(15));
        var departures = rides.Select(ride => ride.Departure).ToArray();
        Assert.Equal(departures.OrderByDescending(departure => departure), departures);
    }

    [Fact]
    public void Early_2025_round_trips_are_listed_newest_first()
    {
        var counts = new int[12];
        counts[0] = 14;
        counts[1] = 12;
        counts[2] = 12;

        var rides = TripPlanner.Plan(2025, counts, new Random(2025));

        Assert.Equal(76, rides.Count);
        Assert.Equal(2025, rides[0].Departure.Year);
        Assert.Equal(3, rides[0].Departure.Month);
        Assert.Equal(1, rides[^1].Departure.Month);
        Assert.DoesNotContain(rides, ride => ride.Departure.Date == new DateTime(2025, 1, 1));
        Assert.Equal(
            rides.Select(ride => ride.Departure),
            rides.Select(ride => ride.Departure).OrderByDescending(departure => departure));
        Assert.Equal(14, rides.Count(ride => ride.Departure.Month == 1 && ride.Departure.Hour < 12));
        Assert.Equal(12, rides.Count(ride => ride.Departure.Month == 2 && ride.Departure.Hour < 12));
        Assert.Equal(12, rides.Count(ride => ride.Departure.Month == 3 && ride.Departure.Hour < 12));
    }

    [Fact]
    public void May_through_december_2023_fills_every_commute_day()
    {
        var counts = new int[12];
        for (var month = 5; month <= 12; month++)
            counts[month - 1] = WorkingCalendar.WorkingDays(2023, month).Count;

        var rides = TripPlanner.Plan(2023, counts, new Random(2023));
        var days = rides.Select(ride => DateOnly.FromDateTime(ride.Departure)).Distinct().ToArray();

        Assert.Equal(169, days.Length);
        Assert.Equal(338, rides.Count);
        Assert.Equal(new DateOnly(2023, 12, 29), DateOnly.FromDateTime(rides[0].Departure));
        Assert.Equal(new DateOnly(2023, 5, 1), DateOnly.FromDateTime(rides[^1].Departure));
        Assert.DoesNotContain(rides, ride => ride.Departure.Date == new DateTime(2023, 5, 5));
        Assert.DoesNotContain(rides, ride => ride.Departure.Month < 5);
    }

    [Fact]
    public void Explicit_days_each_get_one_round_trip_newest_first()
    {
        var rides = TripPlanner.PlanDays(
            [new DateOnly(2024, 1, 8), new DateOnly(2024, 8, 30)],
            new Random(201));

        Assert.Equal(4, rides.Count);
        Assert.Equal(new DateTime(2024, 8, 30), rides[0].Departure.Date);
        Assert.Equal(new DateTime(2024, 8, 30), rides[1].Departure.Date);
        Assert.Equal(new DateTime(2024, 1, 8), rides[2].Departure.Date);
        Assert.Equal(new DateTime(2024, 1, 8), rides[3].Departure.Date);
        Assert.True(rides[0].Departure.Hour >= 17);
        Assert.True(rides[1].Departure.Hour < 12);
        Assert.Equal(rides[1].FromStreet, rides[0].ToStreet);
        Assert.Equal(rides[1].ToStreet, rides[0].FromStreet);
    }

    [Fact]
    public void Zero_round_trips_produce_no_rides() =>
        Assert.Empty(TripPlanner.Plan(2024, new int[12], new Random(1)));

    [Fact]
    public void A_count_above_the_working_days_in_the_month_is_rejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TripPlanner.Plan(2024, Counts(1, 23), new Random(1)));

    private static int[] Counts(int month, int count)
    {
        var counts = new int[12];
        counts[month - 1] = count;
        return counts;
    }

    private static void AssertMorning(DateTime departure)
    {
        Assert.InRange(departure.Hour, 6, 9);
        Assert.Contains(departure.Minute, new[] { 0, 15, 30, 45 });
        Assert.Equal(0, departure.Second);
    }

    private static void AssertEvening(DateTime departure)
    {
        Assert.InRange(departure.Hour, 17, 20);
        Assert.Contains(departure.Minute, new[] { 0, 15, 30, 45 });
        Assert.Equal(0, departure.Second);
    }
}

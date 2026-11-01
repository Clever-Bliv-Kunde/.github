using System.Globalization;

public class RideFormatterTests
{
    [Fact]
    public void A_ride_is_printed_as_one_block()
    {
        var ride = new Ride(
            "Banegårdspladsen 4",
            "Bernstorffsgade 16",
            new DateTime(2024, 8, 8, 6, 45, 0),
            "Lucas",
            1,
            "Completed",
            180,
            "Kolding",
            "København");

        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("da-DK");
        try
        {
            var text = RideFormatter.Format([ride]);
            Assert.Equal(
                """
                Ride route: Banegårdspladsen 4 -> Bernstorffsgade 16
                Departure: 2024-08-08 06:45:00
                Driver: Lucas
                Seats: 1
                State: Completed
                Price: DKK 180
                Route: Kolding -> København

                """,
                text);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void Rides_are_separated_by_a_blank_line()
    {
        var morning = new Ride("Buen 1", "Istedgade 44", new DateTime(2024, 2, 1, 6, 0, 0), "Maja", 1, "Completed", 140, "Kolding", "København");
        var evening = new Ride("Istedgade 44", "Buen 1", new DateTime(2024, 2, 1, 20, 45, 0), "Espen", 1, "Completed", 260, "København", "Kolding");

        var text = RideFormatter.Format([morning, evening]);

        Assert.Contains("Price: DKK 140\nRoute: Kolding -> København\n\nRide route: Istedgade 44 -> Buen 1\n", text);
        Assert.EndsWith("Route: København -> Kolding\n", text);
    }

    [Fact]
    public void No_rides_produce_empty_output() =>
        Assert.Equal("", RideFormatter.Format([]));
}

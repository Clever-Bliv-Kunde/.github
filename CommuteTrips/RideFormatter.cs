using System.Globalization;

public static class RideFormatter
{
    public static string Format(IReadOnlyList<Ride> rides)
    {
        if (rides.Count == 0)
            return "";

        var blocks = rides.Select(ride => string.Join('\n',
        [
            $"Ride route: {ride.FromStreet} -> {ride.ToStreet}",
            $"Departure: {ride.Departure.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}",
            $"Driver: {ride.Driver}",
            $"Seats: {ride.Seats}",
            $"State: {ride.State}",
            $"Price: DKK {ride.PriceDkk.ToString(CultureInfo.InvariantCulture)}",
            $"Route: {ride.FromCity} -> {ride.ToCity}",
        ]));

        return string.Join("\n\n", blocks) + "\n";
    }
}

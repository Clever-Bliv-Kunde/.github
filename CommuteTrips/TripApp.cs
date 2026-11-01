using System.Globalization;

public static class MonthCountParser
{
    public static bool TryParse(string? text, int maximum, out int count)
    {
        count = 0;
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return false;
        if (value < 0 || value > maximum)
            return false;

        count = value;
        return true;
    }
}

public static class SeedOptions
{
    public static bool TryParse(string[] args, out int? seed, out string? error)
    {
        seed = null;
        error = null;
        if (args.Length == 0)
            return true;

        if (args.Length == 2
            && args[0] == "--seed"
            && int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            seed = value;
            return true;
        }

        error = "Usage: CommuteTrips [--seed <int>]";
        return false;
    }
}

public static class TripApp
{
    public static int Run(TextReader input, TextWriter prompts, TextWriter rideOutput, int? seed)
    {
        var counts = new int[12];
        for (var month = 1; month <= 12; month++)
        {
            var maximum = WorkingCalendar.WorkingDays(2024, month).Count;
            var monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);
            if (!TryReadCount(input, prompts, monthName, maximum, out counts[month - 1]))
                return 1;
        }

        var random = seed is int value ? new Random(value) : new Random();
        rideOutput.Write(RideFormatter.Format(TripPlanner.Plan(2024, counts, random)));
        return 0;
    }

    private static bool TryReadCount(
        TextReader input,
        TextWriter prompts,
        string monthName,
        int maximum,
        out int count)
    {
        while (true)
        {
            prompts.WriteLine($"{monthName} 2024 round trips (max {maximum}):");
            var line = input.ReadLine();
            if (line is null)
            {
                prompts.WriteLine($"No input for {monthName}.");
                count = 0;
                return false;
            }

            if (MonthCountParser.TryParse(line, maximum, out count))
                return true;

            prompts.WriteLine($"Enter a whole number from 0 to {maximum}.");
        }
    }
}

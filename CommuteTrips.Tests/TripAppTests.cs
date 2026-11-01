public class MonthCountParserTests
{
    [Theory]
    [InlineData("0", 22, 0)]
    [InlineData("22", 22, 22)]
    [InlineData(" 4 ", 22, 4)]
    public void Whole_numbers_inside_the_month_are_accepted(string text, int maximum, int expected)
    {
        Assert.True(MonthCountParser.TryParse(text, maximum, out var count));
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("nope")]
    [InlineData("-1")]
    [InlineData("23")]
    [InlineData("1.5")]
    [InlineData("2 trips")]
    public void Anything_else_is_rejected(string text) =>
        Assert.False(MonthCountParser.TryParse(text, 22, out _));
}

public class SeedOptionsTests
{
    [Fact]
    public void No_arguments_means_an_unseeded_run()
    {
        Assert.True(SeedOptions.TryParse([], out var seed, out var error));
        Assert.Null(seed);
        Assert.Null(error);
    }

    [Fact]
    public void A_seed_flag_is_parsed()
    {
        Assert.True(SeedOptions.TryParse(["--seed", "42"], out var seed, out var error));
        Assert.Equal(42, seed);
        Assert.Null(error);
    }

    [Theory]
    [InlineData("--seed")]
    [InlineData("--seed", "nope")]
    [InlineData("42")]
    public void Anything_else_is_usage(params string[] args)
    {
        Assert.False(SeedOptions.TryParse(args, out _, out var error));
        Assert.Equal("Usage: CommuteTrips [--seed <int>]", error);
    }
}

public class TripAppTests
{
    [Fact]
    public void Each_month_is_asked_with_its_maximum_and_rides_follow()
    {
        var input = new StringReader(Answers(1, 2));
        var prompts = new StringWriter();
        var rides = new StringWriter();

        var code = TripApp.Run(input, prompts, rides, 7);

        Assert.Equal(0, code);
        Assert.Contains("January 2024 round trips (max 22):", prompts.ToString());
        Assert.Contains("June 2024 round trips (max 19):", prompts.ToString());
        Assert.Contains("December 2024 round trips (max 18):", prompts.ToString());
        Assert.Equal(4, rides.ToString().Split("Ride route:").Length - 1);
    }

    [Fact]
    public void Invalid_counts_are_asked_again()
    {
        var lines = new[] { "nope", "-1", "23", "1" }.Concat(Enumerable.Repeat("0", 11));
        var input = new StringReader(string.Join('\n', lines));
        var prompts = new StringWriter();
        var rides = new StringWriter();

        var code = TripApp.Run(input, prompts, rides, 9);

        Assert.Equal(0, code);
        Assert.Contains("Enter a whole number from 0 to 22.", prompts.ToString());
        Assert.Equal(2, rides.ToString().Split("Ride route:").Length - 1);
    }

    [Fact]
    public void Input_that_ends_early_stops_without_rides()
    {
        var prompts = new StringWriter();
        var rides = new StringWriter();

        var code = TripApp.Run(new StringReader(""), prompts, rides, null);

        Assert.Equal(1, code);
        Assert.Contains("No input for January.", prompts.ToString());
        Assert.Equal("", rides.ToString());
    }

    [Fact]
    public void A_seed_repeats_the_ride_list()
    {
        var first = Run(5);
        var second = Run(5);
        Assert.Equal(first, second);
        Assert.NotEqual(first, Run(6));
    }

    private static string Run(int seed)
    {
        var rides = new StringWriter();
        TripApp.Run(new StringReader(Answers(8, 1)), new StringWriter(), rides, seed);
        return rides.ToString();
    }

    private static string Answers(int month, int count)
    {
        var answers = Enumerable.Repeat("0", 12).ToArray();
        answers[month - 1] = count.ToString();
        return string.Join('\n', answers);
    }
}

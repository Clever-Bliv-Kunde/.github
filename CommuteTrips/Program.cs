using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

if (!SeedOptions.TryParse(args, out var seed, out var error))
{
    Console.Error.WriteLine(error);
    return 1;
}

return TripApp.Run(Console.In, Console.Out, Console.Out, seed);

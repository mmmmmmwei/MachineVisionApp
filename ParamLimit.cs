namespace ConsoleApp1;

public class IntLimit
{
    public int Min { get; }
    public int Max { get; }

    public IntLimit(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public void Validate(string key, int value)
    {
        if (value < Min || value > Max)
            throw new ArgumentOutOfRangeException(
                key,
                $"{key} must be between {Min} and {Max}");
    }

    public int Clamp(int value)
    {
        return Math.Min(Max, Math.Max(Min, value));
    }
}

public class ParamLimit
{
    private static readonly Dictionary<string, IntLimit> _limits
        = new()
    {
        { "ExposureTime", new IntLimit(100, 10000) },
        { "Gain", new IntLimit(0, 20) },
        { "MaxRetryCount", new IntLimit(1, 10) }
    };
}

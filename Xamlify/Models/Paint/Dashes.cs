namespace Xamlify.Models.Paint;

public record Dashes
{
    public float[]? Intervals { get; init; }

    public float Phase { get; init; }
}

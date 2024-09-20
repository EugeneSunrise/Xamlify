namespace Xamlify.Models.Paint;

public record SolidColorBrush : Brush
{
    public ShimSkiaSharp.SKColor Color { get; init; }
}

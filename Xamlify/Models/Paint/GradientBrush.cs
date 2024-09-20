using System.Collections.Generic;

namespace Xamlify.Models.Paint;

public abstract record GradientBrush : Brush
{
    public ShimSkiaSharp.SKShaderTileMode Mode { get; init; }

    public List<GradientStop> GradientStops { get; init; } = new ();
}

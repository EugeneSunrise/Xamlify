using ShimSkiaSharp;
using Xamlify.Models.Paint;
using Xamlify.Models.Resources;

namespace Xamlify.Models.Drawing;
public record PathGeometry : Drawing
{
    public ShimSkiaSharp.SKPaint? Paint { get; }

    public SkiaSharp.SKPath? Geometry { get; }

    public Brush? Brush { get; }

    public Pen? Pen { get; }

    public PathGeometry(
        ShimSkiaSharp.SKPaint? paint = null,
        SkiaSharp.SKPath? geometry = null,
        ResourceDictionary? resources = null)
    {
        Paint = paint;
        Geometry = geometry;

        if (Paint is { } && Geometry is { })
        {
            var isFilled = Paint.Style is ShimSkiaSharp.SKPaintStyle.StrokeAndFill or ShimSkiaSharp.SKPaintStyle.Fill;

            if (isFilled)
            {
                var resourceKey = resources is { } ? $"Brush{resources.BrushCounter++}" : null;

                Brush = Factory.CreateBrush(Paint, Geometry.Bounds, resourceKey);

                if (resources is { } && Brush?.Key is { })
                {
                    resources.Brushes.Add(Brush.Key, (Paint, Brush));
                }
            }

            var isStroked =
                Paint.Style is ShimSkiaSharp.SKPaintStyle.StrokeAndFill or ShimSkiaSharp.SKPaintStyle.Stroke;

            if (isStroked)
            {
                var resourceKey = resources is { } ? $"Pen{resources.PenCounter++}" : null;

                Pen = Factory.CreatePen(Paint, Geometry.Bounds, resourceKey);

                if (resources is { } && Pen?.Key is { })
                {
                    resources.Pens.Add(Pen.Key, (Paint, Pen));
                }
            }
        }
    }
}
﻿using ShimSkiaSharp;
using Xamlify.Models.Containers;
using Xamlify.Models.Drawing;
using Xamlify.Models.Paint;

namespace Xamlify.Models;

internal static class Factory
{
    internal static Svg.Skia.SkiaModel s_model = new Svg.Skia.SkiaModel(new Svg.Skia.SKSvgSettings());

    public static Brush CreateBrush(ShimSkiaSharp.ColorShader colorShader, SkiaSharp.SKRect skBounds, string? key = null)
    {
        var brush = new SolidColorBrush
        {
            Key = key,
            Bounds = skBounds,
            LocalMatrix = null,
            Color = colorShader.Color
        };

        return brush;
    }

    public static LinearGradientBrush CreateBrush(ShimSkiaSharp.LinearGradientShader linearGradientShader, SkiaSharp.SKRect skBounds, string? key)
    {
        var brush = new LinearGradientBrush
        {
            Key = key,
            Bounds = skBounds,
            LocalMatrix = linearGradientShader.LocalMatrix is null
                ? null
                : s_model.ToSKMatrix(linearGradientShader.LocalMatrix.Value),
            Start = s_model.ToSKPoint(linearGradientShader.Start),
            End = s_model.ToSKPoint(linearGradientShader.End),
            Mode = linearGradientShader.Mode
        };

        if (linearGradientShader.Colors is { } && linearGradientShader.ColorPos is { })
        {
            for (var i = 0; i < linearGradientShader.Colors.Length; i++)
            {
                var color = linearGradientShader.Colors[i];
                var offset = linearGradientShader.ColorPos[i];
                brush.GradientStops.Add(new GradientStop { Color = color, Offset = offset });
            }
        }

        return brush;
    }

    public static Brush CreateBrush(ShimSkiaSharp.RadialGradientShader radialGradientShader, SkiaSharp.SKRect skBounds, string? key)
    {
        var brush = new RadialGradientBrush
        {
            Key = key,
            Bounds = skBounds,
            LocalMatrix = radialGradientShader.LocalMatrix is null
                ? null
                : s_model.ToSKMatrix(radialGradientShader.LocalMatrix.Value),
            Center = s_model.ToSKPoint(radialGradientShader.Center),
            Radius = radialGradientShader.Radius,
            Mode = radialGradientShader.Mode
        };

        if (radialGradientShader.Colors is { } && radialGradientShader.ColorPos is { })
        {
            for (var i = 0; i < radialGradientShader.Colors.Length; i++)
            {
                var color = radialGradientShader.Colors[i];
                var offset = radialGradientShader.ColorPos[i];
                brush.GradientStops.Add(new GradientStop { Color = color, Offset = offset });
            }
        }

        return brush;
    }

    public static Brush CreateBrush(ShimSkiaSharp.TwoPointConicalGradientShader twoPointConicalGradientShader, SkiaSharp.SKRect skBounds, string? key)
    {
        var brush = new TwoPointConicalGradientBrush()
        {
            Key = key,
            Bounds = skBounds,
            LocalMatrix = twoPointConicalGradientShader.LocalMatrix is null
                ? null
                : s_model.ToSKMatrix(twoPointConicalGradientShader.LocalMatrix.Value),
            Start = s_model.ToSKPoint(twoPointConicalGradientShader.Start),
            End = s_model.ToSKPoint(twoPointConicalGradientShader.End),
            StartRadius = twoPointConicalGradientShader.StartRadius,
            EndRadius = twoPointConicalGradientShader.EndRadius,
            Mode = twoPointConicalGradientShader.Mode
        };

        if (twoPointConicalGradientShader.Colors is { } && twoPointConicalGradientShader.ColorPos is { })
        {
            for (var i = 0; i < twoPointConicalGradientShader.Colors.Length; i++)
            {
                var color = twoPointConicalGradientShader.Colors[i];
                var offset = twoPointConicalGradientShader.ColorPos[i];
                brush.GradientStops.Add(new GradientStop { Color = color, Offset = offset });
            }
        }

        return brush;
    }

    public static Brush? CreateBrush(ShimSkiaSharp.PictureShader pictureShader, SkiaSharp.SKRect skBounds, string? key = null)
    {
        var brush = new PictureBrush
        {
            Key = key,
            Bounds = skBounds,
            LocalMatrix = s_model.ToSKMatrix(pictureShader.LocalMatrix),
            Picture = new Image(new DrawingImage(new DrawingGroup(pictureShader.Src))),
            CullRect =  pictureShader.Src?.CullRect ?? ShimSkiaSharp.SKRect.Empty,
            Tile = pictureShader.Tile,
            TileMode = pictureShader.TmX
        };

        return brush;
    }

    public static Brush? CreateBrush(ShimSkiaSharp.SKShader skShader, SkiaSharp.SKRect skBounds, string? key = null)
    {
        return skShader switch
        {
            ShimSkiaSharp.ColorShader colorShader => CreateBrush(colorShader, skBounds, key),
            ShimSkiaSharp.LinearGradientShader linearGradientShader => CreateBrush(linearGradientShader, skBounds, key),
            ShimSkiaSharp.RadialGradientShader radialGradientShader => CreateBrush(radialGradientShader, skBounds, key),
            ShimSkiaSharp.TwoPointConicalGradientShader twoPointConicalGradientShader => CreateBrush(twoPointConicalGradientShader, skBounds, key),
            ShimSkiaSharp.PictureShader pictureShader => CreateBrush(pictureShader, skBounds, key),
            _ => null
        };
    }

    public static Brush? CreateBrush(ShimSkiaSharp.SKPaint skPaint, SkiaSharp.SKRect skBounds, string? key)
    {
        return CreateBrush( 
            new ColorShader(
            new SKColor(
                skPaint.Color.Value.Red, 
                skPaint.Color.Value.Green, 
                skPaint.Color.Value.Blue,
                skPaint.Color.Value.Alpha),
            SKColorSpace.Srgb), skBounds, key);
    }
    
    public static Pen? CreatePen(ShimSkiaSharp.SKPaint skPaint, SkiaSharp.SKRect skBounds, string? key)
    {

        var pen = new Pen
        {
            Key = key,
            Bounds = skBounds,
            Brush = CreateBrush(skPaint, skBounds,null),
            StrokeWidth = skPaint.StrokeWidth,
            StrokeCap = skPaint.StrokeCap,
            StrokeJoin = skPaint.StrokeJoin,
            StrokeMiter = skPaint.StrokeMiter,
            Dashes = skPaint.PathEffect is ShimSkiaSharp.DashPathEffect(var intervals, var phase) { Intervals: { } }
                ? new Dashes { Intervals = intervals, Phase = phase }
                : null
        };

        return pen;
    }
}

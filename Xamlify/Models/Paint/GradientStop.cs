﻿using Xamlify.Models.Resources;

namespace Xamlify.Models.Paint;

public record GradientStop : Resource
{
    public float Offset { get; init; }

    public ShimSkiaSharp.SKColor Color { get; init; }
}

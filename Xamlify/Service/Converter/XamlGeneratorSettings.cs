﻿using Xamlify.Models.Resources;

namespace Xamlify.Service.Converter;

public record XamlGeneratorSettings
{
    public string NewLine { get; init; } = "\r\n";

    public bool UseCompatMode { get; init; } = false;

    public bool AddTransparentBackground { get; init; } = false;

    public bool ReuseExistingResources { get; init; } = false;

    public bool TransformGeometry { get; init; } = false;

    public bool WriteResources { get; init; } = false;

    public ResourceDictionary? Resources { get; init; }
}

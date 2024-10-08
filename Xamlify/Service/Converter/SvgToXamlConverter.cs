﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Xamlify.Service.Converter;

public class SvgToXamlConverter
{
    public string NewLine { get; set; } = "\r\n";

    public bool UseCompatMode { get; set; }

    public bool AddTransparentBackground { get; set; }

    public bool ReuseExistingResources { get; set; }

    public  bool TransformGeometry { get; set; }

    public Models.Resources.ResourceDictionary? Resources { get; set; }

    
    public string ToPathGeometry(ShimSkiaSharp.SKPicture? skPicture, string? key = "MyGeometryGroup")
    {
        var drawingGroup = new Models.Drawing.GeometryGroup(skPicture, Resources, key);

        var context = new XamlGeneratorSettings
        {
            NewLine = NewLine,
            UseCompatMode = UseCompatMode,
            AddTransparentBackground = AddTransparentBackground,
            ReuseExistingResources = ReuseExistingResources,
            TransformGeometry = TransformGeometry,
            WriteResources = false,
            Resources = Resources
        };

        return new XamlGenerator().GenerateGeometryGroup(drawingGroup, context);
    }
    public string ToXamlDrawingGroup(ShimSkiaSharp.SKPicture? skPicture, string? key = null)
    {
        var drawingGroup = new Models.Drawing.DrawingGroup(skPicture, Resources, key);

        var context = new XamlGeneratorSettings
        {
            NewLine = NewLine,
            UseCompatMode = UseCompatMode,
            AddTransparentBackground = AddTransparentBackground,
            ReuseExistingResources = ReuseExistingResources,
            TransformGeometry = TransformGeometry,
            WriteResources = false,
            Resources = Resources
        };

        return new XamlGenerator().GenerateDrawingGroup(drawingGroup, context);
    }

    public string ToXamlImage(ShimSkiaSharp.SKPicture? skPicture, string? key = null)
    {
        var drawingGroup = new Models.Drawing.DrawingGroup(skPicture, Resources);
        var drawingImage = new Models.Drawing.DrawingImage(drawingGroup);
        var image = new Models.Containers.Image(drawingImage, key);

        var context = new XamlGeneratorSettings
        {
            NewLine = NewLine,
            UseCompatMode = UseCompatMode,
            AddTransparentBackground = AddTransparentBackground,
            ReuseExistingResources = ReuseExistingResources,
            TransformGeometry = TransformGeometry,
            WriteResources = true,
            Resources = Resources
        };

        return new XamlGenerator().GenerateImage(image, context, null);
    }

    public string ToXamlStyles(string content, bool generateImage = false, bool generatePreview = true)
    {
        var results = new List<(string Path, string Key, Models.Resources.Resource Resource)>();
 
      
            try
            {
                var svg = new Svg.Skia.SKSvg();
                svg.FromSvg(content);

                var key = $"_{CreateKey("inputItem.Name")}";
                if (generateImage)
                {
                    var drawingGroup = new Models.Drawing.DrawingGroup(svg.Model, Resources);
                    var drawingImage = new Models.Drawing.DrawingImage(drawingGroup);
                    var image = new Models.Containers.Image(drawingImage, key);
                    results.Add(("inputItem.Name", key, image));
                }
                else
                {
                    var drawingGroup = new Models.Drawing.DrawingGroup(svg.Model, Resources, key);
                    results.Add(("inputItem.Name", key, drawingGroup));
                }
            }
            catch
            {
                // ignored
            }
        

        var resources = results.Select(x => x.Resource).ToList();
        var styles = new Models.Containers.Styles(resources, generateImage, generatePreview);

        var context = new XamlGeneratorSettings
        {
            NewLine = NewLine,
            UseCompatMode = UseCompatMode,
            AddTransparentBackground = AddTransparentBackground,
            ReuseExistingResources = ReuseExistingResources,
            TransformGeometry = TransformGeometry,
            WriteResources = false,
            Resources = Resources
        };

        return new XamlGenerator().GenerateStyles(styles, context);
    }

    public virtual string CreateKey(string path)
    {
        string name = Path.GetFileNameWithoutExtension(path);
        string key = name.Replace("-", "_");
        return $"_{key}";
    }

    public virtual string Format(string xml)
    {
        try
        {
            var sb = new StringBuilder();
            sb.Append($"<Root");
            sb.Append(UseCompatMode
                ? $" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
                : $" xmlns=\"https://github.com/avaloniaui\"");
            sb.Append($" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            sb.Append($">");
            sb.Append(xml);
            sb.Append($"</Root>");

            using var ms = new MemoryStream();
            using var writer = new XmlTextWriter(ms, Encoding.UTF8);
            var document = new XmlDocument();
            document.LoadXml(sb.ToString());
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.IndentChar = ' ';
            document.WriteContentTo(writer);
            writer.Flush();
            ms.Flush();
            ms.Position = 0;
            using var sReader = new StreamReader(ms);
            var formatted = sReader.ReadToEnd();

            var lines = formatted.Split(NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var inner = lines.Skip(1).Take(lines.Length - 2).Select(x => x.Substring(2, x.Length - 2));
            return string.Join(NewLine, inner);
        }
        catch
        {
            // ignored
        }

        return "";
    }
}

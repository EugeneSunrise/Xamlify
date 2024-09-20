using System;
using Avalonia;
using Svg.Model;
using System.IO;
using Xamlify.Models;
using Avalonia.Threading;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamlify.Service.Converter;
using System.Collections.Generic;
using Avalonia.Controls.ApplicationLifetimes;

namespace Xamlify.ViewModels;

public partial class XamlifyWindowViewModel : ViewModelBase
{
    private string _myText;
    public string MyText
    {
        get => _myText;
        set => SetProperty(ref _myText, value);
    }

    private string _svgContent;

    public string SvgContent
    {
        get => _svgContent;
        set => SetProperty(ref _svgContent, value);
    }

    private EType _convertType;

    public EType ConvertType
    {
        get => _convertType;
        set
        {
            SetProperty(ref _convertType, value);
            Task.Run(async () => await Update());
        }
    }

    private SvgViewModel _svg;

    public SvgViewModel? Svg
    {
        get => _svg;
        private set => SetProperty(ref _svg, value);
    }

    private SkiaSharp.SKPicture? _picture;

    public SkiaSharp.SKPicture? Picture
    {
        get => _picture;
        private set => SetProperty(ref _picture, value);
    }

    public XamlifyWindowViewModel()
    {
    }

    public async void Drop(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                var svgPaths = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories);
                var svgzPaths = Directory.EnumerateFiles(path, "*.svgz", SearchOption.AllDirectories);
                Drop(svgPaths);
                Drop(svgzPaths);
                continue;
            }

            var extension = Path.GetExtension(path);
            switch (extension.ToLower())
            {
                case ".svg":
                case ".svgz":
                {
                    await using var stream = File.OpenRead(path);
                    var ms = await LoadFromStream(stream, path);
                    var name = Path.GetFileName(path);
                    await Add(ms, name);
                    break;
                }
            }
        }
    }

    private async Task<Stream> LoadFromStream(Stream stream, string name)
    {
        var extension = Path.GetExtension(name);
        var memoryStream = new MemoryStream();

        if (extension == "svgz")
        {
            await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            await gzipStream.CopyToAsync(memoryStream);
        }
        else
        {
            await stream.CopyToAsync(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private async Task Update()
    {
        await Task.Run(() =>
        {
            Svg = new SvgViewModel();
            Picture = Svg.FromSvg(SvgContent, new DrawAttributes());
        });
        
        var xaml = await ToXaml();

        MyText = xaml;
        await SetClipboard(xaml); 
    }

    private async Task Add(Stream stream, string name)
    {
        using var reader = new StreamReader(stream);
        SvgContent = await reader.ReadToEndAsync();
        await Update();
    }

    private async Task<string> ToXaml()
    {
        return await Task.Run(async () =>
        {
            if (Svg is null) return string.Empty;
            var converter = new SvgToXamlConverter()
            {
                UseCompatMode = true,
                AddTransparentBackground = false,
                ReuseExistingResources = false,
                TransformGeometry = false,
                Resources = new Models.Resources.ResourceDictionary()
            };
            return ConvertType switch
            {
                EType.Path => converter.Format(converter.ToPathGeometry(Svg.Model)),
                EType.DrawingImage => converter.Format(converter.ToXamlImage(Svg.Model)),
                EType.DrawingGroup => converter.Format(converter.ToXamlDrawingGroup(Svg.Model)),
                _ => throw new ArgumentOutOfRangeException()
            };
        });
    }
    
    private async Task SetClipboard(string? xaml)
    {
        if (xaml is not { })
        {
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    if (lifetime.MainWindow?.Clipboard is {} clipboard)
                    {
                        await clipboard.SetTextAsync(xaml);
                    }
                }
            }
            catch
            {
                // ignored
            }
        });
    }
}
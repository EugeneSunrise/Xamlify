using Xamlify.Models.Drawing;
using Xamlify.Models.Resources;

namespace Xamlify.Models.Containers;

public record Image : Resource
{
    public DrawingImage? Source { get; }

    public Image(DrawingImage? source = null, string? key = null)
    {
        Key = key;
        Source = source;
    }
}

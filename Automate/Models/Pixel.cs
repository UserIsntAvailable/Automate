using System.Drawing;

namespace Automate.Models {

    /// <summary>
    /// A class declaring a pixel on a bitmap
    /// </summary>
    public record Pixel((int X, int Y) Position, Color Color);
}

using System.Drawing;

namespace Automate.Models {

    /// <summary>
    /// A class declaring a pixel on a bitmap
    /// </summary>
    public record Pixel((int X, int Y) Position, Color Color) {

        #region Public Properties

        /// <summary>
        /// The X position on the bitmap
        /// </summary>
        public int X => Position.X;

        /// <summary>
        /// The Y position on the bitmap
        /// </summary>
        public int Y => Position.Y;
        #endregion
    }
}

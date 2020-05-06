using System;
using System.Drawing;

namespace Automate.Models {

    /// <summary>
    /// A class declaring a pixel on a bitmap
    /// </summary>
    public class Pixel : IEquatable<Pixel> {

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Pixel((int X, int Y) position, Color color) {

            Position = position; Color = color;
        }
        #endregion

        #region IEquatable Implementation

        public bool Equals(Pixel other) {

            return this.X == other.X && this.Y == other.Y && this.Color == other.Color;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The X position on the bitmap
        /// </summary>
        public int X => Position.X;

        /// <summary>
        /// The Y position on the bitmap
        /// </summary>
        public int Y => Position.Y;

        /// <summary>
        /// The color of the pixel
        /// </summary>
        public Color Color = new Color();
        #endregion

        #region Override Methods

        public override int GetHashCode() {

            int hash = 21;

            hash = hash * 11 + X.GetHashCode();
            hash = hash * 11 + Y.GetHashCode();
            hash = hash * 11 + Color.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj) {

            if (!(obj is Pixel)) return false;

            return Equals((Pixel)obj);
        }

        public override string ToString() => $"{X} | {Y} | R:{Color.R} G:{Color.G} B:{Color.B}";
        #endregion

        /// <summary>
        /// The position of the of the pixel on the bitmap
        /// </summary>
        internal (int X, int Y) Position { get; set; }
    }
}

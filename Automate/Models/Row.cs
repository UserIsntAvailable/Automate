using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Automate.Models {

    /// <summary>
    /// A custom collection declaring a row on a bitmap
    /// </summary>
    public class Row : IEquatable<Row>, IEnumerable<Pixel> {

        #region Private Fields

        /// <summary>
        /// The pixels that define this row
        /// </summary>
        private readonly Pixel[] _pixels;
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Row(Pixel[] pixels) {

            this._pixels = pixels;
        }
        #endregion

        #region Interfaces Implementations

        #region IEquatable Implementation

        /// <summary>
        /// Check if the two rows are exactly the same
        /// </summary>
        /// <param name="other">Your comparison object</param>
        public bool Equals(Row other) {

            if (this.Length != other.Length) return false;

            if (!this._pixels.SequenceEqual(other._pixels)) return false;

            return true;
        }
        #endregion

        #region Enumerable Implementation

        /// <summary>
        /// Default Enumerator
        /// </summary>
        /// <returns><see cref="Enumerable"/></returns>
        public IEnumerator<Pixel> GetEnumerator() {

            foreach (var pixel in _pixels) {

                yield return pixel;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
        #endregion

        #region Index and Range Implementations

        #region Index Implementation

        /// <summary>
        /// Gets or sets a value on <see cref="_pixels"/> prop
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel this[int index] {

            get { return _pixels[index]; }

            set { _pixels[index] = value; }
        }
        #endregion

        #region Range Implementation

        public Row Slice(int start, int length) {

            var slice = new Pixel[length];

            Array.Copy(_pixels, start, slice, 0, length);

            return new Row(slice);
        }
        #endregion
        #endregion

        #region Public Properties

        /// <summary>
        /// The width of this row
        /// </summary>
        public int Length => _pixels.Length;
        #endregion

        #region Public Methods

        /// <summary>
        /// Chech if the two rows are only equals by colors presents on the same order skipping pixels position
        /// </summary>
        /// <param name="other">Your comparison object</param>
        /// <param name="tolerance">The tolerance ( check README.md for more information )</param>
        public bool Equals(Row other, decimal tolerance) {

            foreach (var (thisPixel, otherPixel) in this._pixels.Zip(other._pixels)) {

                if (EuclideanColorDistanceMetric(thisPixel.Color, otherPixel.Color) > tolerance) {

                    return false;
                }
            }

            return true;
        }

        #region Override Methods

        public override bool Equals(object obj) {

            if (!(obj is Row)) return false;

            return Equals((Row)obj);
        }

        public override int GetHashCode() => _pixels.GetHashCode();
        #endregion
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the distance between two colors
        /// </summary>
        /// <returns></returns>
        private static decimal EuclideanColorDistanceMetric(Color c1, Color c2) {

            // color distance
            var distanceSquared =
                  (c1.R - c2.R) * (c1.R - c2.R)
                + (c1.G - c2.G) * (c1.G - c2.G)
                + (c1.B - c2.B) * (c1.B - c2.B);

            // 195075 = (255 * 255) + (255 * 255) + (255 + 255)
            return (decimal)(distanceSquared / 195075d);
        }
        #endregion
    }
}

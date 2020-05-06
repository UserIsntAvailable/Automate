using System.Linq;
using System.Drawing;
using Automate.Models;
using System.Collections.Generic;

namespace Automate {

    /// <summary>
    /// A class for Extentions of <see cref="Row"/> or <see cref="Row"/>[]
    /// </summary>
    public static class RowExtentions {

        /// <summary>
        /// Gets all distinct colors on a <see cref="Row"/>
        /// </summary>
        /// <param name="row">This row object</param>
        /// <returns>An array of <see cref="Color"/></returns>
        public static Color[] GetColors(this Row row) {

            List<Color> colors = new List<Color>();

            foreach (var pixel in row) {

                colors.Add(pixel.Color);
            }

            return colors.Distinct().ToArray();
        }

        /// <summary>
        /// Gets all distinct colors on a <see cref="Row"/>[]
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static Color[] GetColors(this Row[] rows) {

            List<Color> colors = new List<Color>();

            foreach (var row in rows) {

                foreach (var pixel in row) {

                    colors.Add(pixel.Color);
                }
            }

            return colors.Distinct().ToArray();
        }

        /// <summary>
        /// Transform a <see cref="Row"/>[] in a <see cref="Bitmap"/> object
        /// </summary>
        /// <param name="rows">The <see cref="Row"/>[]</param>
        /// <returns>A <see cref="Bitmap"/> object</returns>
        public static Bitmap ToBitmap(this Row[] rows) {

            Bitmap image = new Bitmap(rows[0].Length, rows.Length);

            foreach (var row in rows) {

                foreach (var pixel in row) {

                    image.SetPixel(pixel.X, pixel.Y, pixel.Color);
                }
            }

            return image;
        }

        /// <summary>
        /// Check if this row contains a color specified
        /// </summary>
        /// <param name="row">This row object</param>
        /// <param name="color">The <see cref="Color"/> object</param>
        /// <returns>A boolean</returns>
        public static bool ContainsColor(this Row row, Color color) {

            return row.Any(p => p.Color == color);
        }
    }
}

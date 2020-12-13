using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Automate.Models;

namespace Automate {

    /// <summary>
    /// A class for Extentions of <see cref="Row"/> or <see cref="IEnumerable{Row}"/>
    /// </summary>
    public static class RowExtentions {

        /// <summary>
        /// Gets all distinct colors on a <see cref="Row"/>
        /// </summary>
        /// <param name="row">This <see cref="Row"/></param>
        /// <returns>An array of distincts colors</returns>
        public static Color[] GetColors(this Row row) {

            List<Color> colors = new List<Color>();

            foreach (var pixel in row) {

                colors.Add(pixel.Color);
            }

            return colors.Distinct().ToArray();
        }

        /// <summary>
        /// Gets all distinct colors on a <see cref="IEnumerable{Row}"/>
        /// </summary>
        /// <param name="rows">This <see cref="IEnumerable{Row}"/></param>
        /// <returns>An array of distincts colors</returns>
        public static Color[] GetColors(this IEnumerable<Row> rows) {

            return rows.SelectMany(row => row.GetColors()).Distinct().ToArray();
        }

        /// <summary>
        /// Transform a <see cref="IEnumerable{Row}"/> in a <see cref="Bitmap"/>
        /// </summary>
        /// <param name="rows">This <see cref="IEnumerable{Row}"/></param>
        /// <returns>A <see cref="Bitmap"/></returns>
        public static Bitmap ToBitmap(this IEnumerable<Row> rows) {

            Bitmap image = new Bitmap(rows.ElementAt(0).Length, rows.Count());

            foreach (var row in rows) {

                foreach (var pixel in row) {

                    // Pretty slow....
                    image.SetPixel(pixel.X, pixel.Y, pixel.Color);
                }
            }

            return image;
        }

        /// <summary>
        /// Check if this row contains a certain color
        /// </summary>
        /// <param name="row">This <see cref="Row"/></param>
        /// <param name="color">A <see cref="Color"/></param>
        /// <returns>A boolean indicating if the <paramref name="color"/> was found</returns>
        public static bool ContainsColor(this Row row, Color color) {

            return row.Any(p => p.Color == color);
        }
    }
}

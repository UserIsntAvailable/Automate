using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using Automate.Models;

namespace Automate {

    /// <summary>
    /// A class for <see cref="Bitmap"/> image processing extentions
    /// </summary>
    public static class BitmapExtentions {

        #region Public Methods

        /// <summary>
        /// Convert a <see cref="Bitmap"/> into a <see cref="ReadOnlyCollection{Row}"/>
        /// </summary>
        /// <param name="bmp">Your bitmap object</param>
        /// <returns>A <see cref="ReadOnlyCollection{Row}"/></returns>
        public static unsafe ReadOnlyCollection<Row> ToRowCollection(this Bitmap bmp) {

            // Returned
            Row[] rows = new Row[bmp.Height];

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Pointer to the first line ( horizontal ) of the bitmap
            byte* scan0 = (byte*)bmpData.Scan0;

            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

            for (int y = 0; y < bmpData.Height; y++) {

                var newRow = new Row(new Pixel[bmp.Width]);

                // The current line pointer on the memory data
                byte* currentLine = scan0 + (y * bmpData.Stride);

                for (int x = 0; x < bmpData.Width * bytesPerPixel; x += bytesPerPixel) {

                    var pixelIndex = x / bytesPerPixel;

                    newRow[pixelIndex] = new Pixel(
                        (pixelIndex, y),
                        Color.FromArgb(
                            currentLine[x + 2], currentLine[x + 1], currentLine[x])
                        );
                }

                rows[y] = newRow;
            }

            // The system memory will dispose the bitmap if is not used anymore 
            bmp.UnlockBits(bmpData);

            return Array.AsReadOnly(rows);
        }

        /// <summary>
        /// Check if the two bitmaps are exactly the same
        /// </summary>
        /// <param name="thisBitmap">This bitmap</param>
        /// <param name="other">Your comparison</param>
        public static bool EqualTo(this Bitmap thisBitmap, Bitmap other) {

            if (thisBitmap.Width != other.Width || thisBitmap.Height != other.Height) return false;

            if (!thisBitmap.ToRowCollection().SequenceEqual(other.ToRowCollection())) return false;

            return true;
        }

        /// <summary>
        /// Check if the two bitmaps are the same with a tolerance value
        /// </summary>
        /// <param name="thisBitmap">This bitmap</param>
        /// <param name="other">Your comparison</param>
        /// <param name="tolerence">The tolerance percentage ( check README.md for more information )</param>
        public static bool EqualTo(this Bitmap thisBitmap, Bitmap other, decimal tolerence) {

            throw new NotImplementedException("I will do it later :sure:");
        }

        /// <summary>
        /// Try to search a <paramref name="template"/> into <paramref name="thisBitmap"/>
        /// </summary>
        /// <param name="thisBitmap">This bitmap object</param>
        /// <param name="template">The bitmap template object</param>
        /// <param name="p">The first pixel that matches in the two bitmaps</param>
        /// <returns>A boolean that indicates if the <paramref name="template"/> was found on <paramref name="thisBitmap"/></returns>
        public static bool TryMatchBitmaps(this Bitmap thisBitmap, Bitmap template, out Pixel p) {

            return thisBitmap.TryMatchBitmaps(template, 0, out p);
        }

        /// <summary>
        /// Try to search a <paramref name="template"/> into <paramref name="thisBitmap"/> with a tolerance value
        /// </summary>
        /// <param name="thisBitmap">This bitmap object</param>
        /// <param name="template">The bitmap template object</param>
        /// <param name="tolerance">The tolerance percentage ( check README.md for more information )</param>
        /// <param name="p">The first pixel that matches in the two bitmaps</param>
        /// <returns>A boolean that indicates if the <paramref name="template"/> was found on <paramref name="thisBitmap"/></returns>
        public static bool TryMatchBitmaps(this Bitmap thisBitmap, Bitmap template, decimal tolerance, out Pixel p) {

            // Maybe I need to resize the bitmaps for fast search
            ReadOnlyCollection<Row> thisRowArray = thisBitmap.ToRowCollection();
            ReadOnlyCollection<Row> templateRowArray = template.ToRowCollection();

            int templateHeigth = templateRowArray.Count;
            int templateWidth = templateRowArray[0].Length;

            // For loop each row on thisRowArray
            for (int y1 = 0; y1 < thisRowArray.Count - templateHeigth; y1++) {

                // For loop each pixel on each row inside thisRowArray
                for (int x1 = 0; x1 < thisRowArray[y1].Length - templateWidth; x1++) {

                    // thisRow cropped for compare with templateRow
                    var thisRowCropped = thisRowArray[y1][x1..(templateWidth + x1)];

                    // If the thisRowCropped == templateRowArray[0] ( only for color orders )
                    // check the others rows
                    if (thisRowCropped.EqualsByColors(templateRowArray[0], tolerance)) {

                        var allMatch = true;

                        // For loop each row on templateRowArray ( starting from second line )
                        for (int y2 = 1; y2 < templateHeigth; y2++) {

                            allMatch = thisRowArray[y1 + y2][x1..(templateWidth + x1)]
                                .EqualsByColors(templateRowArray[y2], tolerance);

                            // If at least one row doesn't match
                            // break this loop
                            if (!allMatch) break;
                        }

                        // First pixel that matched
                        p = thisRowCropped[0];

                        return true;
                    }
                }
            }

            // Nothing found
            p = new Pixel((0, 0), Color.Empty);

            return false;
        }
        #endregion
    }
}

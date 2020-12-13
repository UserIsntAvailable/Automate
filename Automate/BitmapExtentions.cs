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
        /// Convert a <see cref="Bitmap"/> into a <see cref="Row"/>[]
        /// </summary>
        /// <param name="bmp">Your bitmap object</param>
        /// <returns>An array of <see cref="Row"/>/returns>
        public static unsafe ReadOnlyCollection<Row> ToRowArray(this Bitmap bmp) {

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
        /// <returns>A boolean</returns>
        public static bool EqualTo(this Bitmap thisBitmap, Bitmap other) {

            if (thisBitmap.Width != other.Width || thisBitmap.Height != other.Height) return false;

            if (!thisBitmap.ToRowArray().SequenceEqual(other.ToRowArray())) return false;

            return true;
        }

        /// <summary>
        /// Check if the two bitmaps are the same with a tolerance value
        /// </summary>
        /// <param name="thisBitmap">This bitmap</param>
        /// <param name="other">Your comparison</param>
        /// <param name="tolerence">The tolerance percentage ( check README.md for more information )</param>
        /// <returns>A boolean</returns>
        public static bool EqualTo(this Bitmap thisBitmap, Bitmap other, int tolerence) {

            throw new NotImplementedException("I will do it later :sure:");
        }

        /// <summary>
        /// Try to search a bitmap template into thisBitmap
        /// </summary>
        /// <param name="thisBitmap">This bitmap object</param>
        /// <param name="template">The bitmap template object</param>
        /// <param name="p">The first pixel that matches in the two bitmaps</param>
        /// <returns>A boolean that indicates if the template was found on this thisBitmap</returns>
        public static bool TryMatchBitmaps(this Bitmap thisBitmap, Bitmap template, out Pixel p) {

            return thisBitmap.TryMatchBitmaps(template, 0, out p);
        }

        /// <summary>
        /// Try to search a bitmap template into thisBitmap
        /// </summary>
        /// <param name="thisBitmap">This bitmap object</param>
        /// <param name="template">The bitmap template object</param>
        /// <param name="tolerance">The tolerance percentage ( check README.md for more information )</param>
        /// <param name="p">The first pixel that matches in the two bitmaps</param>
        /// <returns>A boolean that indicates if the template was found on this thisBitmap</returns>
        public static bool TryMatchBitmaps(this Bitmap thisBitmap, Bitmap template, int tolerance, out Pixel p) {

            // Maybe I need to resize the bitmaps for fast search
            ReadOnlyCollection<Row> thisRowArray = thisBitmap.ToRowArray();
            ReadOnlyCollection<Row> templateRowArray = template.ToRowArray();

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
                    if (thisRowCropped.Equals(templateRowArray[0], tolerance)) {

                        var allMatch = true;

                        // For loop each row on templateRowArray ( starting from second line )
                        for (int y2 = 1; y2 < templateHeigth; y2++) {

                            allMatch = thisRowArray[y1 + y2][x1..(templateWidth + x1)]
                                .Equals(templateRowArray[y2], tolerance);

                            // If at least one row doesn't match
                            // break this loop
                            if (!allMatch) {

                                break;
                            }
                        }

                        // First pixel found
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

        #region Obsolete Methods

        // I only don't want to remove the old code

        [Obsolete]
        /// <summary>
        /// Convert a <see cref="Bitmap"/> into a byte[y][x][rgb]
        /// </summary>
        /// <param name="bmp">Your bitmap object</param>
        /// <returns>Bitmap data in byte[y][x][rbg]</returns>
        internal static unsafe byte[][][] ToByteArray(this Bitmap bmp) {

            // Returned
            byte[][][] bmpArray = new byte[bmp.Height][][];

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Pointer to the first line ( horizontal ) of the bitmap
            byte* scan0 = (byte*)bmpData.Scan0;

            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

            // For some weird reason bmpData.Stride is rounding the output causing an indexOutRange on the second for loop
            int bmpDataStride = bmpData.Width * bytesPerPixel;

            for (int y = 0; y < bmpData.Height; y++) {

                bmpArray[y] = new byte[bmp.Width][];

                // The current line pointer on the memory data
                byte* currentLine = scan0 + (y * bmpDataStride);

                for (int x = 0; x < bmpDataStride; x += bytesPerPixel) {

                    bmpArray[y][x / bytesPerPixel] = new byte[] {

                        //currentLine[x + 3] // If I would need alpha I only need to uncomment it
                        currentLine[x + 2],  //r
                        currentLine[x + 1],  //g
                        currentLine[x],      //b
                    };
                }
            }

            // The system memory will dispose the bitmap if is not used anymore 
            bmp.UnlockBits(bmpData);

            return bmpArray;
        }

        [Obsolete]
        /// <summary>
        /// Verify if the second line is present on the first line
        /// </summary>
        internal static bool IsLineEqualTo(this byte[][][] mainBmpArray, byte[][][] subBmpArray) {

            int mainBmpWidth = mainBmpArray.GetLength(1);
            int subBmpWidth = subBmpArray.GetLength(1);

            if (subBmpWidth > mainBmpWidth) {

                throw new ArgumentException("Your subBmpArray is bigger than your main. Maybe you inverse orders?", nameof(subBmpArray));
            }

            while (true) {

                var mainBmpResized = mainBmpArray[0].Take(subBmpWidth + 1);

                if (mainBmpResized.Equals(subBmpArray)) {

                    return true;
                }

                else {

                    mainBmpResized = mainBmpArray[0].Skip(1);

                    if (mainBmpResized.Count() < subBmpWidth) {

                        return false;
                    }
                }
            }
        }
        #endregion
    }
}

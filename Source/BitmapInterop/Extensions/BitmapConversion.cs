#region Licence and Terms
// DotImaging Framework
// https://github.com/dajuric/dot-imaging
//
// Copyright © Darko Jurić, 2014-2018
// darko.juric2@gmail.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DotImaging
{
    /// <summary>
    /// Provides conversion extension methods between generic image and <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public static class BitmapConversionExtensions
    {
        #region Conversion from Bitmap

        private static TColor[,] toArray<TColor>(Bitmap bmp)
            where TColor: struct, IColor
        {
            var bmpData = bmp.LockBits(ImageLockMode.ReadOnly);

            var arr = new TColor[bmp.Height, bmp.Width];
            using (var img = arr.Lock())
            {
                Copy.UnsafeCopy2D(bmpData.Scan0, img.ImageData, bmpData.Stride, img.Stride, bmpData.Height);
            }

            bmp.UnlockBits(bmpData);
            return arr;
        }

        /// <summary>
        /// Converts a bitmap to an image (copies data). 
        /// </summary>
        /// <param name="bmp">Input bitmap.</param>
        /// <returns>2D array.</returns>
        public static Array ToArray(this Bitmap bmp)
        {
            Array arr = null;
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    arr = toArray<Gray<byte>>(bmp);
                    break;
                case PixelFormat.Format16bppGrayScale:
                     arr = toArray<Gray<short>>(bmp);
                    break;
                case PixelFormat.Format24bppRgb:
                    arr = toArray<Bgr<byte>>(bmp);
                    break;
                case PixelFormat.Format32bppArgb:
                    arr = toArray<Bgra<byte>>(bmp);
                    break;
                case PixelFormat.Format48bppRgb:
                    arr = toArray<Bgr<short>>(bmp);
                    break;
                case PixelFormat.Format64bppArgb:
                    arr = toArray<Bgra<short>>(bmp);
                    break; 
                default:
                    throw new NotSupportedException();
            }

            return arr;
        }

        /// <summary>
        /// Converts the specified bitmap into Bgr managed image.
        /// </summary>
        /// <param name="bitmap">Bgr, Bgra or Gray type bitmap.</param>
        /// <returns>Bgr image or null if conversion can not be performed.</returns>
        public static Bgr<byte>[,] ToBgr(this Bitmap bitmap)
        {
            var arr = bitmap.ToArray();
            return arr.ToBgr();
        }

        /// <summary>
        /// Converts the specified bitmap into Bgra managed image.
        /// </summary>
        /// <param name="bitmap">Bgra, Bgr or Gray type bitmap.</param>
        /// <returns>Bgra image or null if conversion can not be performed.</returns>
        public static Bgra<byte>[,] ToBgra(this Bitmap bitmap)
        {
            var arr = bitmap.ToArray();
            return arr.ToBgra();
        }

        #endregion

        #region Conversion To Bitmap

        private static Bitmap toBitmap(IImage img, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(img.Width, img.Height, pixelFormat);
            var bmpData = bmp.LockBits(ImageLockMode.WriteOnly);
            Copy.UnsafeCopy2D(img.ImageData, bmpData.Scan0, img.Stride, bmpData.Stride, bmpData.Height);
            bmp.UnlockBits(bmpData);

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                bmp.SetGrayscalePalette();

            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Gray<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format8bppIndexed);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Gray<short>> img)
        {
            return toBitmap(img, PixelFormat.Format16bppGrayScale);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgr<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgra<byte>> img)
        {
            return toBitmap(img, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgr<short>> img)
        {
            return toBitmap(img, PixelFormat.Format48bppRgb);
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Image<Bgra<short>> img)
        {
            return toBitmap(img, PixelFormat.Format64bppArgb);
        }


        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Gray<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format8bppIndexed); 
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Gray<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format16bppGrayScale);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgr<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format24bppRgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgra<byte>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format32bppArgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgr<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format48bppRgb);
            }
            return bmp;
        }

        /// <summary>
        /// Converts an image to an bitmap.
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this Bgra<short>[,] img)
        {
            Bitmap bmp = null;
            using (var uImg = img.Lock())
            {
                bmp = toBitmap(uImg, PixelFormat.Format64bppArgb);
            }
            return bmp;
        }

        #endregion

        #region Cast to Bitmap

        private static Bitmap asBitmap(IImage img, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(img.Width, img.Height, img.Stride, pixelFormat, img.ImageData);

            if (pixelFormat == PixelFormat.Format8bppIndexed)
                bmp.SetGrayscalePalette();

            return bmp;
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Gray<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format8bppIndexed);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Gray<short>> img)
        {
            return asBitmap(img, PixelFormat.Format16bppGrayScale);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgr<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgra<byte>> img)
        {
            return asBitmap(img, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgr<short>> img)
        {
            return asBitmap(img, PixelFormat.Format48bppRgb);
        }

        /// <summary>
        /// Casts an image to an bitmap.
        /// <para>Notice that GDI+ does not support bitmaps which stride is not 4.</para>
        /// </summary>
        /// <param name="img">Input image.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AsBitmap(this Image<Bgra<short>> img)
        {
            return asBitmap(img, PixelFormat.Format64bppArgb);
        }

        #endregion

        #region Misc
 
        /// <summary>
        /// Replaces color palette entries with grayscale intensities (256 entries).
        /// </summary>
        /// <param name="image">The 8-bpp grayscale image.</param>
        public static void SetGrayscalePalette(this Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException("The provided image must have 8bpp pixel format.");

            var palette = image.Palette;
            for (int i = 0; i < (Byte.MaxValue + 1); i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }

            image.Palette = palette;
        }

        /// <summary>
        /// Lock a <see cref="System.Drawing.Bitmap"/> into system memory.
        /// </summary>
        /// <param name="bmp">Bitmap to lock.</param>
        /// <param name="imageLockMode">Specifies the access level.</param>
        /// <returns>Bitmap data.</returns>
        public static BitmapData LockBits(this Bitmap bmp, ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
        {
            return bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), imageLockMode, bmp.PixelFormat);
        }

        #endregion
    }
}

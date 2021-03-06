﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageSegmentation
{
    public static class ImageBinaryConverter
    {

        /*
         * Returns byte array containing pixels (red, green, blue, alpha);        
         * You can then access the Red, Green, Blue and Alpha components as follows:
         * byte blue = bytes[index];
         * byte green = bytes[index + 1];
         * byte red = bytes[index + 2];
         * byte alpha = bytes[index + 3];
        */
        public static byte[] ImageToBytes(BitmapSource source)
        {
            int bytesPerPixel = (int)Math.Ceiling(source.Format.BitsPerPixel / 8.0);
            int stride = source.PixelWidth * bytesPerPixel;
            int size = source.PixelHeight * stride;
            
            byte[] result = new byte[size];

            source.CopyPixels(result, stride, 0);

            return result;
        }

        public static BitmapSource BytesToImage(int width,
                                                int height,
                                                PixelFormat format,
                                                BitmapPalette palette,
                                                Array pixels,
                                                int stride)
        {
            return BitmapSource.Create(width, height, 96, 96, format, palette, pixels, stride);
        }

        public static BitmapSource BytesToImage(ImageProperties image, Array pixels)
        {
            return BitmapSource.Create(image.Width, image.Height, 96, 96, image.Image.Format, image.Image.Palette, pixels, image.Stride);
        }

        public static BitmapSource ImageToFormat(BitmapSource source, PixelFormat format)
        {
            if (source.Format != format)
            {
                return new FormatConvertedBitmap(source, format, null, 0);
            }
            return source;
        }

        public static byte[] getHalftoneBgr32(byte[] pixels) 
        {
            byte[] result = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i = i + 4 )
            {
                byte color = Convert.ToByte((pixels[i + 2] + pixels[i + 1] + pixels[i]) / 3);

                result[i] = color;
                result[i + 1] = color;
                result[i + 2] = color;
                result[i + 3] = result[i + 3];
            }
            return result;
        }
    }
}

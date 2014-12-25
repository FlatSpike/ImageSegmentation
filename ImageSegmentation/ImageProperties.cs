using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Clustering;

namespace ImageSegmentation
{
    public class ImageProperties
    {
        public ImageProperties(BitmapSource image)
        {
            Image = image;
            Bytes = ImageBinaryConverter.ImageToBytes(Image);
            Vector = Clustering.Vector.GetVectors(Bytes, BytesPerPixel);
        }

        public Vector getPixel(int x, int y)
        {
            if (!CheckBorders(x, y)) 
            {
                // TODO: Add error handler
                return null;
            }
            return new Vector(Vector[x + y * Width]);
        }

        public BitmapSource Image { get; private set; }

        public Vector[] Vector { get; private set; }

        public byte[] Bytes { get; private set; }

        public int Width { get { return Image.PixelWidth; } }

        public int Height { get { return Image.PixelHeight; } }

        public int BytesPerPixel { get { return (int)Math.Ceiling(Image.Format.BitsPerPixel / 8.0); } }

        public int Stride { get { return Width * BytesPerPixel; } }

        private bool CheckBorders(int x, int y) { return (0 < x && x < Width) && (0 < y && y < Height); }
    }
}

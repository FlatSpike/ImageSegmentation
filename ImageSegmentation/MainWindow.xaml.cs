using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageSegmentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty CurrentImageProperty = DependencyProperty.Register("CurrentImage", typeof(BitmapSource), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ButtonOpenClick(object sender, RoutedEventArgs e)
        {
            CreateOpenFileDialog();
        }

        private void ButtonProcessingClick(object sender, RoutedEventArgs e)
        {
            if (imageProperties.Image != null)
            {
                testKMeanClustering();
                //testMeanShiftClustering();
            }
            else
            {
                // TODO: Add error handle
            }
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (imageProperties.Image != null)
            {
                Point position = e.GetPosition(Image);
                Color color = PickColor(position.X, position.Y);

            }
        }

        // For testing
        // TODO: Remove
        private void testKMeanClustering()
        {
            Image2.Source = imageProperties.Image;

            List<Clustering.Vector> centroids = new List<Clustering.Vector>();
            centroids.Add(new Clustering.Vector(0, 0, 0, 255));
            centroids.Add(new Clustering.Vector(255, 255, 255, 255));

            List<Clustering.Vector> vectors = new List<Clustering.Vector>(imageProperties.Vector);
            List<Clustering.Cluster> clusters = Clustering.Clustering.kMeansClustering(vectors, Clustering.Criteria.EuclideanDistance, centroids);
            Clustering.Clustering.ApplyClustering(clusters, vectors);

            imageProperties = new ImageProperties(
                ImageBinaryConverter.BytesToImage(imageProperties, Clustering.Vector.GetBytes(vectors.ToArray())));
            SetValue(CurrentImageProperty, imageProperties.Image);
        }

        private void testMeanShiftClustering()
        {
            Image2.Source = imageProperties.Image;

            List<Clustering.Vector> vectors = new List<Clustering.Vector>(imageProperties.Vector);
            List<Clustering.Cluster> clusters = Clustering.Clustering.MeanShiftClustering(vectors, Clustering.Criteria.EuclideanDistance, 2);
            Clustering.Clustering.ApplyClustering(clusters, vectors);

            imageProperties = new ImageProperties(
                ImageBinaryConverter.BytesToImage(imageProperties, Clustering.Vector.GetBytes(vectors.ToArray())));
            SetValue(CurrentImageProperty, imageProperties.Image);
        }

        private void CreateOpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog
            {
                Filter = GetImageFilter(),
                FilterIndex = 1,
                Multiselect = false
            };

            bool? userClickedOk = openFileDialog1.ShowDialog();

            if (userClickedOk == true)
            {
                LoadImage(openFileDialog1.FileName);
                SetValue(CurrentImageProperty, imageProperties.Image);
            }
        }

        private void LoadImage(string fileName)
        {
            imageProperties = new ImageProperties(new BitmapImage(new Uri(fileName)));
        }

        /// http://www.codeproject.com/Tips/255626/A-FileDialog-Filter-generator-for-all-supported-im
        /// <summary>
        /// Get the Filter string for all supported image types.
        /// This can be used directly to the FileDialog class Filter Property.
        /// </summary>
        /// <returns></returns>
        public string GetImageFilter()
        {
            StringBuilder allImageExtensions = new StringBuilder();
            string separator = "";
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            Dictionary<string, string> images = new Dictionary<string, string>();
            foreach (ImageCodecInfo codec in codecs)
            {
                allImageExtensions.Append(separator);
                allImageExtensions.Append(codec.FilenameExtension);
                separator = ";";
                images.Add(string.Format("{0} Files: ({1})", codec.FormatDescription, codec.FilenameExtension),
                           codec.FilenameExtension);
            }
            StringBuilder sb = new StringBuilder();
            if (allImageExtensions.Length > 0)
            {
                sb.AppendFormat("{0}|{1}", "All Images", allImageExtensions.ToString());
            }
            // images.Add("All Files", "*.*");
            foreach (KeyValuePair<string, string> image in images)
            {
                sb.AppendFormat("|{0}|{1}", image.Key, image.Value);
            }
            return sb.ToString();
        }

        /// http://www.codeproject.com/Articles/36848/WPF-Image-Pixel-Color-Picker-Element
        /// <summary>
        /// Picks the color at the position specified.
        /// </summary>
        /// <param name="x">The x coordinate in WPF pixels.</param>
        /// <param name="y">The y coordinate in WPF pixels.</param>
        /// <returns>The image pixel color at x,y position.</returns>
        public Color PickColor(double x, double y)
        {
            BitmapSource bitmapSource = imageProperties.Image;
            if (bitmapSource != null)
            { // Get color from bitmap pixel.
                // Convert coopdinates from WPF pixels to Bitmap pixels
                // and restrict them by the Bitmap bounds.
                x *= bitmapSource.PixelWidth / ActualWidth;
                if ((int)x > bitmapSource.PixelWidth - 1)
                    x = bitmapSource.PixelWidth - 1;
                else if (x < 0)
                    x = 0;
                y *= bitmapSource.PixelHeight / ActualHeight;
                if ((int)y > bitmapSource.PixelHeight - 1)
                    y = bitmapSource.PixelHeight - 1;
                else if (y < 0)
                    y = 0;

                // Lee Brimelow approach (http://thewpfblog.com/?p=62).
                //byte[] pixels = new byte[4];
                //CroppedBitmap cb = new CroppedBitmap(bitmapSource, 
                //                   new Int32Rect((int)x, (int)y, 1, 1));
                //cb.CopyPixels(pixels, 4, 0);
                //return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);

                // Alternative approach
                if (bitmapSource.Format == PixelFormats.Indexed4)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth * 
                                  bitmapSource.Format.BitsPerPixel + 3) / 4;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), 
                                                           pixels, stride, 0);
                    return bitmapSource.Palette.Colors[pixels[0] >> 4];
                }
                else if (bitmapSource.Format == PixelFormats.Indexed8)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth * 
                                  bitmapSource.Format.BitsPerPixel + 7) / 8;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), 
                                                           pixels, stride, 0);
                    return bitmapSource.Palette.Colors[pixels[0]];
                }
                else
                {
                    byte[] pixels = new byte[4];
                    int stride = (bitmapSource.PixelWidth * 
                                  bitmapSource.Format.BitsPerPixel + 7) / 8;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), 
                                                           pixels, stride, 0);

                    return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                }
            }
            return new Color();
        }

        private ImageProperties imageProperties;
    }
}

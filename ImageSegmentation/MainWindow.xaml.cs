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
            if (CurrentImage != null)
            {
                testKMeanClustering();
                //testMeanShiftClustering();
            }
            else
            {
                // TODO: Add error handle
            }
        }

        // For testing
        private void testKMeanClustering()
        {
            Image2.Source = CurrentImage;
            byte[] pixels = ImageBinaryConverter.ImageToBytes(CurrentImage);
            _vectors = Clustering.Vector.GetVectors(pixels, BytesPerPixel);

            List<Clustering.Vector> centroids = new List<Clustering.Vector>();
            centroids.Add(new Clustering.Vector(0, 0, 0, 255));
            centroids.Add(new Clustering.Vector(255, 255, 255, 255));

            List<Clustering.Cluster> clusters = Clustering.Clustering.kMeansClustering(new List<Clustering.Vector>(_vectors), Clustering.Criteria.EuclideanDistance, centroids);
            List<Clustering.Vector> vectors = new List<Clustering.Vector>(_vectors);
            Clustering.Clustering.ApplyClustering(clusters, vectors);
            _vectors = vectors.ToArray();
            pixels = Clustering.Vector.GetBytes(_vectors);

            CurrentImage = ImageBinaryConverter.BytesToImage(CurrentImage.PixelWidth, CurrentImage.PixelHeight,
                CurrentImage.Format, CurrentImage.Palette, pixels, Stride);
        }

        private void testMeanShiftClustering()
        {
            Image2.Source = CurrentImage;
            byte[] pixels = ImageBinaryConverter.ImageToBytes(CurrentImage);
            _vectors = Clustering.Vector.GetVectors(pixels, BytesPerPixel);

            List<Clustering.Cluster> clusters = Clustering.Clustering.MeanShiftClustering(new List<Clustering.Vector>(_vectors), Clustering.Criteria.EuclideanDistance, 2);
            List<Clustering.Vector> vectors = new List<Clustering.Vector>(_vectors);
            Clustering.Clustering.ApplyClustering(clusters, vectors);
            _vectors = vectors.ToArray();
            pixels = Clustering.Vector.GetBytes(_vectors);

            CurrentImage = ImageBinaryConverter.BytesToImage(CurrentImage.PixelWidth, CurrentImage.PixelHeight,
                CurrentImage.Format, CurrentImage.Palette, pixels, Stride);
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
            }
        }

        private void LoadImage(string fileName)
        {
            CurrentImage = new BitmapImage(new Uri(fileName));
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

        public BitmapSource CurrentImage
        {
            get { return (BitmapSource)GetValue(CurrentImageProperty); }
            set
            {
                SetValue(CurrentImageProperty, value);
            }
        }

        private int BytesPerPixel
        {
            get { return (int) Math.Ceiling(CurrentImage.Format.BitsPerPixel/8.0); }
        }

        private int Stride
        {
            get { return CurrentImage.PixelWidth * BytesPerPixel; }
        }

        private Clustering.Vector[] _vectors;
    }
}

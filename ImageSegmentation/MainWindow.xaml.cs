using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            RefreshPanel();
        }

        private void ButtonOpenClick(object sender, RoutedEventArgs e)
        {
            CreateOpenFileDialog();
        }

        private void ButtonProcessingClick(object sender, RoutedEventArgs e)
        {
            if (ImageProperties.Image != null)
            {
                Cursor = Cursors.Wait;
                switch (ComboBoxMethod.SelectedIndex)
                {
                    case 0:
                        meanShiftClustering((int)ScaleSlider.Value);
                        break;
                    case 1:
                        if (kMeanCentroidsType.SelectedIndex == 0)
                        {
                            kMeanClustering(_kMeanCentroids);
                        }
                        else
                        {
                            // TODO: Add random centroids selection
                        }
                        break;
                }
                Cursor = Cursors.Arrow;
            }
            else
            {
                // TODO: Add error handle
            }
            _kMeanCentroids.Clear();
        }

        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            ColorsList.Items.Clear();
            PointCount.Content = "0";
            _kMeanCentroids.Clear();
        }
        
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ImageProperties.Image != null)
            {
                if (ComboBoxMethod.SelectedIndex == 1) 
                {
                    if (kMeanCentroidsType.SelectedIndex == 0)
                    {
                        Point position = e.GetPosition(Image);
                        Color color = PickColor(position.X, position.Y);
                        createMenuItem(color, position.X, position.Y);
                        PointCount.Content = ColorsList.Items.Count;
                    }
                }
            }
        }

        private void ComboBoxMethod_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            HideSettings();
            switch (ComboBoxMethod.SelectedIndex)
            { 
                case 0:
                    showMeanShiftSettings();
                    break;
                case 1:
                    showKMeanSettings();
                    _kMeanCentroids = new List<Clustering.Vector>();
                    break;
            }
        }

        // For testing
        // TODO: Remove
        private void kMeanClustering(List<Clustering.Vector> centroids)
        {
            List<Clustering.Vector> vectors = new List<Clustering.Vector>(ImageProperties.Vector);
            List<Clustering.Cluster> clusters = Clustering.Clustering.kMeansClustering(vectors, Clustering.Criteria.EuclideanDistance, centroids);
            Clustering.Clustering.ApplyClusteringColor(clusters, vectors);

            ImageProperties = new ImageProperties(
                ImageBinaryConverter.BytesToImage(ImageProperties, Clustering.Vector.GetBytes(vectors.ToArray())));
        }

        private void meanShiftClustering(int scale)
        {
            List<Clustering.Vector> vectors = new List<Clustering.Vector>(ImageProperties.Vector);
            List<Clustering.Cluster> clusters = Clustering.Clustering.MeanShiftClustering(vectors, Clustering.Criteria.EuclideanDistance, scale);
            Clustering.Clustering.ApplyClustering(clusters, vectors);

            ImageProperties = new ImageProperties(
                ImageBinaryConverter.BytesToImage(ImageProperties, Clustering.Vector.GetBytes(vectors.ToArray())));
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
            ImageProperties = new ImageProperties(new BitmapImage(new Uri(fileName)));
            OriginalImage.Source = CurrentImage;
            ColorsList.Items.Clear();

        }

        private void createMenuItem(Color color, double x, double y)
        {
            Button button = new Button();
            button.Background = new SolidColorBrush(color);
            button.Width = 150;
            button.Height = 10;
            button.Click += new System.Windows.RoutedEventHandler(this.menuButton_Click);
            _kMeanCentroids.Add(new Clustering.Vector(color.B, color.G, color.R, color.A));
            ColorsList.Items.Add(button);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            ColorsList.Items.Remove(sender);
            PointCount.Content = ColorsList.Items.Count;
        }

        private void HideSettings()
        {
            SettingsBox.Visibility = Visibility.Hidden;
            foreach (UIElement children in Settings.Children)
            {
                children.Visibility = Visibility.Collapsed;
            }
        }

        private void showKMeanSettings()
        {
            SettingsBox.Visibility = Visibility.Visible;
            KMeanSettings.Visibility = Visibility.Visible;
        }

        private void showMeanShiftSettings()
        {
            SettingsBox.Visibility = Visibility.Visible;
            MeanShiftSettings.Visibility = Visibility.Visible;
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
            BitmapSource bitmapSource = ImageProperties.Image;
            if (bitmapSource != null)
            { // Get color from bitmap pixel.
                // Convert coopdinates from WPF pixels to Bitmap pixels
                // and restrict them by the Bitmap bounds.

                x *= bitmapSource.PixelWidth / Image.ActualWidth;
                if ((int)x > bitmapSource.PixelWidth - 1)
                    x = bitmapSource.PixelWidth - 1;
                else if (x < 0)
                    x = 0;
                y *= bitmapSource.PixelHeight / Image.ActualHeight;
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

        public BitmapSource CurrentImage
        {
            get { return (BitmapSource)GetValue(CurrentImageProperty); }
        }

        private ImageProperties ImageProperties
        {
            get { return _imageProperties; }
            set
            {
                _imageProperties = value;
                SetValue(CurrentImageProperty, _imageProperties.Image);
                RefreshPanel();
            }
        }

        private void RefreshPanel()
        {
            if (_imageProperties == null)
            {
                foreach (UIElement child in MainPanel.Children)
                {
                    child.IsEnabled = false;
                }
                OpenImageButton.IsEnabled = true;
            }
            else
            {
                foreach (UIElement child in MainPanel.Children)
                {
                    child.IsEnabled = true;
                }
            }

        }

        private List<Clustering.Vector> _kMeanCentroids;
        private ImageProperties _imageProperties;

    }
}

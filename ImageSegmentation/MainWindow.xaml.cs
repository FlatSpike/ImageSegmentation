using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageSegmentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            createOpenFileDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (currentImage != null)
            {
                byte[] pixels = getPixels(currentImage, 4);
                vectors = Clustering.Vector.getVectors(pixels, 4);
            }
            else
            {
                // TODO: Add error handle
            }
        }

        private void createOpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            openFileDialog1.Filter =
                "All files (*.*)|*.*|" +
                "BMP (*.BMP;*.RLE;*.DIB)|*.BMP;*.RLE;*.DIB|" +
                "JPEG (*.JPG;*.JPEG;*.JPE)|*.JPG;*.JPEG;*.JPE|" +
                "PNG (*.PNG)|*.PNG";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            bool? userClickedOK = openFileDialog1.ShowDialog();

            if (userClickedOK == true)
            {
                currentImage = new BitmapImage(new Uri(openFileDialog1.FileName));
                Image.Source = currentImage;
            }
        }

        // For RGB image numParams must be is 4, if it is
        //  pixels[count + 0] - blue,
        //  pixels[count + 1] - green,
        //  pixels[count + 2] - red,
        //  pixels[count + 3] - alpha,
        // where count - number of the current pixel
        private byte[] getPixels(BitmapImage image, int numParams) 
        {
            int stride = currentImage.PixelWidth * numParams;
            int size = currentImage.PixelHeight * stride;

            byte[] result = new byte[size];

            image.CopyPixels(result, stride, 0);

            return result;
        }

        private Clustering.Vector[] vectors;
        private BitmapImage currentImage;
    }
}

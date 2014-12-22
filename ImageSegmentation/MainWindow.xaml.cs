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

        private BitmapImage currentImage;
    }
}

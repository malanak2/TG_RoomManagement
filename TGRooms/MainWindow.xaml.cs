using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TGRooms
{
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32", SetLastError = true)]
        public static extern void FreeConsole();

        public bool isDebug = false;
        DataView dataView;
        private readonly double aspectRatio = 16.0 / 9.0;
        public MainWindow()
        {
            InitializeComponent();
            if (isDebug)
            {
                AllocConsole();
            }
            dataView = new DataView(this);
            dataView.Visibility = Visibility.Hidden;

        }


        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.Height = e.NewSize.Width / aspectRatio;
            }
            else if (e.HeightChanged)
            {
                this.Width = e.NewSize.Height * aspectRatio;
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isDebug)
            {
                FreeConsole();
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Mistnosti_Click(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("Changing Screen to DataView with Mistnost");
            dataView.Visibility = Visibility.Visible;
            dataView.Set_Data_Type(Choice.Mistnosti);
            this.Visibility = Visibility.Hidden;
        }

        private void Spravci_Click(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("Changing Screen to DataView with Spravci");
            dataView.Visibility = Visibility.Visible;
            dataView.Set_Data_Type(Choice.Spravci);

            this.Visibility = Visibility.Hidden;
        }
    }
}
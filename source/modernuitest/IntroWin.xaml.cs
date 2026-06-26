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
using System.Windows.Shapes;
using ModernWpf;
using static System.Net.Mime.MediaTypeNames;

namespace modernuitest
{
    /// <summary>
    /// Interaction logic for IntroWin.xaml
    /// </summary>
    public partial class IntroWin : Window
    {
        public IntroWin()
        {
            ModernWpf.ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light; // or Light, or null for system
            InitializeComponent();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = Product.SelectedItem as ComboBoxItem;
            string text = selectedItem.Content.ToString();

            if (text == "Delta Robot") { 
            var Window = new MainWindow();
            Window.Show();
            this.Close();
            }else if(text =="4DoF Serial Manipulator")
            {
                var Window = new Serial();
                Window.Show();
                this.Close();
            }
        }

        private void Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = Product.SelectedItem as ComboBoxItem;
            string text = selectedItem.Content.ToString();

            if (text == "Delta Robot")
            {
                ProductIMG.Source = new BitmapImage(new Uri("/newFolder/delta.png", UriKind.Relative));
            }
            else if (text == "4DoF Serial Manipulator")
            {
                ProductIMG.Source = new BitmapImage(new Uri("/newFolder/serial.png", UriKind.Relative));
            }

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Nxt_Click(object sender, RoutedEventArgs e)
        {
            Product.SelectedIndex++;
        }

        private void Prv_Click(object sender, RoutedEventArgs e)
        {
            if (Product.SelectedIndex > 0)
            {
                Product.SelectedIndex--;
            }
        }
    }
}

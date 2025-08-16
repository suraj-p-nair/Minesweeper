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

namespace MineSweeper.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }
        private void StartEasy(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new GamePage(9,9,10));
        }
        private void StartMedium(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new GamePage(16,16,40));
        }
        private void StartHard(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new GamePage(16,30,60));
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

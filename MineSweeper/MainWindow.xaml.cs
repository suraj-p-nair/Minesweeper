using MineSweeper.Pages;
using System.Windows;
using System.Windows.Controls;

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new StartPage();
        }
    }
}

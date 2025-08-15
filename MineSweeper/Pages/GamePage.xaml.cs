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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
            CreateBoard();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                Button btn = new()
                {
                    Content = $"{i}"
                };
                MineField.Children.Add(btn);
            }
            StartTimer();

        }
        private async void StartTimer()
        {
            int time = 0;
            while (true)
            {
                
                await Task.Delay(1000);
                time++;
                Timer.Text = time.ToString();
            }
        }
    }
}

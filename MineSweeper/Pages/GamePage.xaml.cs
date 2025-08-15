using MineSweeper.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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
            StartTimer();
        }

        Image flag = new()
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/Assets/flag.png"))
        };
        private void CreateBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                var cellData = new CellData
                {
                    DisplayValue = $"Cell {i}"
                };

                Button btn = new()
                {
                    Content = "",
                    Tag = cellData,
                    FontFamily = new FontFamily("Segoe UI Emoji")
                };

                btn.Click += RevealCell;
                btn.MouseRightButtonDown += FlagCell;

                MineField.Children.Add(btn);
            }
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
        private void RevealCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                if (!cell.IsRevealed && !cell.IsFlagged)
                {
                    cell.IsRevealed = true;
                    btn.Content = cell.DisplayValue;
                    btn.IsEnabled = false;
                }
            }
        }

        private void FlagCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                cell.IsFlagged = !cell.IsFlagged;
                btn.Content = cell.IsFlagged ?  GetFlag(): "";
                
            }
        }

        private static Image GetFlag()
        {
            return new Image { Source = FlagSource };
        }

        private static readonly BitmapImage FlagSource = new(
            new Uri("pack://application:,,,/Assets/flag.png")
        );
    }
}

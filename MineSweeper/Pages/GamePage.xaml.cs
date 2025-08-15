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
        private int[] MinePositions = [];
        private int MineCount = 0;
        public GamePage()
        {
            InitializeComponent();
            GenerateMinePositions();
            CreateBoard();
            StartTimer();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                var cellData = new CellData
                {
                    DisplayValue = $"Cell {i}",
                    IsMine = MinePositions.Contains(i)
                };

                Button btn = new()
                {
                    Content = cellData.IsMine ? "1" : "",
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
                    btn.Content = cell.IsMine ? GetMine() : cell.DisplayValue;
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
                MineCounter.Text = cell.IsFlagged ? (++MineCount).ToString() : (--MineCount).ToString();
            }
        }

        private static Image GetFlag()
        {
            return new Image { Source = FlagSource };
        }
        private static Image GetMine()
        {
            return new Image { Source = MineSource };
        }

        private static readonly BitmapImage FlagSource = new(
            new Uri("pack://application:,,,/Assets/flag.png")
        );
        private static readonly BitmapImage MineSource = new(
            new Uri("pack://application:,,,/Assets/mine.png")
        );
        private void GenerateMinePositions()
        {
            Random rng = new Random();
            MinePositions = [.. Enumerable.Range(0, 100)
                                        .OrderBy(_ => rng.Next())
                                        .Take(10)];
            Console.WriteLine(string.Join(", ", MinePositions));
            MineCount = 10;
            MineCounter.Text = MineCount.ToString();
        }
    }
}

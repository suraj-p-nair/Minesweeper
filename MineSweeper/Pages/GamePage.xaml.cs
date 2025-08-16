using MineSweeper.Models;
using System;
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
        private CellData[,] MineField = new CellData[10,10] ;
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
                    IsMine = MinePositions.Contains(i),
                    Row = i / 10,
                    Col = i % 10
                };

                MineField[i/10,i%10] = cellData;

                Button btn = new()
                {
                    Tag = cellData,
                    FontFamily = new FontFamily("Segoe UI Emoji")
                };

                btn.Click += RevealCell;
                btn.MouseRightButtonDown += FlagCell;

                MineFieldGrid.Children.Add(btn);
            }
            CalculateAdjacentMineCount();
            UpdateCellMineCount();
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
                    btn.Content = cell.IsMine ? GetMine() : cell.AdjacentMines;
                    btn.IsEnabled = false;
                }
            }
        }

        private void FlagCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                cell.IsFlagged = !cell.IsFlagged;
                btn.Content = cell.IsFlagged ? GetFlag() : "";
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

        private void CalculateAdjacentMineCount()
        {
            foreach (Button btn in MineFieldGrid.Children)
            {
                CellData cellData = (CellData)btn.Tag;

                if (cellData.IsMine)
                {
                    UpdateAdjacentCells(cellData);
                }
            }
        }

        private void UpdateAdjacentCells(CellData currentCell)
        {
            int currRow = currentCell.Row;
            int currCol = currentCell.Col;
            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int nr = currRow + dr;
                    int nc = currCol + dc;

                    if (nr >= 0 && nr < 10 && nc >= 0 && nc < 10)
                    {
                        if (!MineField[nr, nc].IsMine)
                        {
                            MineField[nr, nc].AdjacentMines++;
                        }
                    }
                }
            }

        }

        private void UpdateCellMineCount()
        {
            foreach (Button btn in MineFieldGrid.Children)
            {
                CellData cellData = (CellData)btn.Tag;

                btn.Content = cellData.AdjacentMines;
            }
        }
    }
}

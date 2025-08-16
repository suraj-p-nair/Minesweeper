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
                    FontFamily = new FontFamily("Segoe UI Emoji"),
                    FontSize = 12
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

                    if (cell.IsMine)
                    {
                        btn.Content = GetMine();
                    }
                    else if (cell.AdjacentMines > 0)
                    {
                        btn.Content = cell.AdjacentMines.ToString();
                        btn.Foreground = GetMineCountColor(cell.AdjacentMines);
                    }
                    else
                    {
                        RevealValidCells(cell);
                    }

                    btn.IsEnabled = false;
                }
            }
        }

        private Brush GetMineCountColor(int count)
        {
            return count switch
            {
                1 => Brushes.Blue,
                2 => Brushes.Green,
                3 => Brushes.Red,
                4 => Brushes.DarkBlue,
                5 => Brushes.Brown,
                6 => Brushes.Cyan,
                7 => Brushes.Black,
                8 => Brushes.Gray,
                _ => Brushes.Black
            };
        }

        private void FlagCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                cell.IsFlagged = !cell.IsFlagged;
                btn.Content = cell.IsFlagged ? GetFlag() : "";
                MineCounter.Text = cell.IsFlagged ? (--MineCount).ToString() : (++MineCount).ToString();
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

        private void RevealValidCells(CellData startCell)
        {
            Queue<CellData> queue = new();
            HashSet<CellData> visited = new();

            //Add current cell into revealing queue and mark as visited
            queue.Enqueue(startCell);
            visited.Add(startCell);

            /* While the queue is not empty:
             * 1. Take the next cell from the queue.
             * 2. Find its button and mark it as revealed.
             * 3. If it has a number (AdjacentMines > 0):
             *      - Display the number with its color.
             *      - Do NOT enqueue its neighbors (stop expansion here).
             * 4. If it has no adjacent mines (AdjacentMines == 0):
             *      - Display it as blank.
             *      - For each neighbor:
             *          - If the neighbor is within bounds, not a mine, not revealed, and not visited:
             *              - Add it to visited.
             *              - Add it to the queue to reveal later.
             */
            while (queue.Count > 0)
            {
                CellData cell = queue.Dequeue();

                Button btn = GetButtonForCell(cell);
                if (btn != null)
                {
                    cell.IsRevealed = true;
                    btn.IsEnabled = false;

                    if (cell.AdjacentMines > 0)
                    {
                        btn.Content = cell.AdjacentMines.ToString();
                        btn.Foreground = GetMineCountColor(cell.AdjacentMines);
                        continue;
                    }
                    else
                    {
                        btn.Content = "";
                    }
                }

                foreach (int dr in new[] { -1, 0, 1 })
                {
                    foreach (int dc in new[] { -1, 0, 1 })
                    {
                        if (dr == 0 && dc == 0) continue;

                        int nr = cell.Row + dr;
                        int nc = cell.Col + dc;

                        if (nr >= 0 && nr < 10 && nc >= 0 && nc < 10)
                        {
                            CellData neighbor = MineField[nr, nc];

                            if (!neighbor.IsMine && !neighbor.IsRevealed && !visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }
        }

        private Button GetButtonForCell(CellData cell)
        {
            foreach (Button btn in MineFieldGrid.Children)
            {
                if (btn.Tag == cell)
                    return btn;
            }
            return null;
        }

    }
}

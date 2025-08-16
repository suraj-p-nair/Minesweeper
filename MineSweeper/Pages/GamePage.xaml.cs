using MineSweeper.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Linq;

namespace MineSweeper.Pages
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private int _rowCount;
        private int _colCount;
        private int[] MinePositions = [];
        private int _mineCount;
        private CellData[,] MineField;

        public GamePage(int RowCount, int ColCount, int MineCount) { 
            _rowCount = RowCount;
            _colCount = ColCount;
            _mineCount = MineCount;
            MineField = new CellData[_rowCount, _colCount];
            InitializeComponent();
            MineFieldGrid.Rows = RowCount;
            MineFieldGrid.Columns = ColCount;
            GenerateMinePositions();
            CreateBoard();
            StartTimer();
            MineCounter.Text = _mineCount.ToString();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < _rowCount * _colCount; i++)
            {
                var cellData = new CellData
                {
                    DisplayValue = $"Cell {i}",
                    IsMine = MinePositions.Contains(i),
                    Row = i / _colCount,
                    Col = i % _colCount
                };

                MineField[i / _colCount, i % _colCount] = cellData;

                Button btn = new()
                {
                    Tag = cellData,
                    FontFamily = new FontFamily("Segoe UI Emoji"),
                    FontSize = 12,
                    Background = Brushes.LightGray
                };

                btn.Click += RevealCell;
                btn.MouseRightButtonDown += FlagCell;
                btn.MouseDoubleClick += HandleMouseClick;

                MineFieldGrid.Children.Add(btn);
            }

            CalculateAdjacentMineCount();
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
                        btn.IsEnabled = false;
                    }
                    else if (cell.AdjacentMines > 0)
                    {
                        btn.Content = cell.AdjacentMines.ToString();
                        btn.Foreground = GetMineCountColor(cell.AdjacentMines);
                        btn.Background = Brushes.White;
                        btn.FocusVisualStyle = null;
                        btn.IsEnabled = true;
                    }
                    else
                    {
                        RevealValidCells(cell);
                    }
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
                if ((!cell.IsFlagged && _mineCount == 0) || cell.IsRevealed)
                {
                    return;
                }

                cell.IsFlagged = !cell.IsFlagged;
                btn.Content = cell.IsFlagged ? GetFlag() : "";
                MineCounter.Text = cell.IsFlagged ? (--_mineCount).ToString() : (++_mineCount).ToString();
            }
        }

        private static Image GetFlag() => new Image { Source = FlagSource };
        private static Image GetMine() => new Image { Source = MineSource };

        private static readonly BitmapImage FlagSource = new(
            new Uri("pack://application:,,,/Assets/flag.png")
        );
        private static readonly BitmapImage MineSource = new(
            new Uri("pack://application:,,,/Assets/mine.png")
        );

        private void GenerateMinePositions()
        {
            Random rng = new Random();
            MinePositions = [.. Enumerable.Range(0, _rowCount * _colCount)
                                        .OrderBy(_ => rng.Next())
                                        .Take(_mineCount)];
            Console.WriteLine(string.Join(", ", MinePositions));
            
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

                    if (nr >= 0 && nr < _rowCount && nc >= 0 && nc < _colCount)
                    {
                        if (!MineField[nr, nc].IsMine)
                        {
                            MineField[nr, nc].AdjacentMines++;
                        }
                    }
                }
            }
        }

        private void RevealValidCells(CellData startCell)
        {
            Queue<CellData> queue = new();
            HashSet<CellData> visited = new();

            queue.Enqueue(startCell);
            visited.Add(startCell);

            while (queue.Count > 0)
            {
                CellData cell = queue.Dequeue();
                Button btn = GetButtonForCell(cell);

                if (btn != null)
                {
                    cell.IsRevealed = true;

                    if (cell.AdjacentMines > 0)
                    {
                        btn.Content = cell.AdjacentMines.ToString();
                        btn.Foreground = GetMineCountColor(cell.AdjacentMines);
                        btn.Background = Brushes.White;
                        btn.FocusVisualStyle = null;
                        btn.IsEnabled = true;
                        continue;
                    }
                    else
                    {
                        btn.Content = "";
                        btn.IsEnabled = false;
                    }
                }

                foreach (int dr in new[] { -1, 0, 1 })
                {
                    foreach (int dc in new[] { -1, 0, 1 })
                    {
                        if (dr == 0 && dc == 0) continue;

                        int nr = cell.Row + dr;
                        int nc = cell.Col + dc;

                        if (nr >= 0 && nr < _rowCount && nc >= 0 && nc < _colCount)
                        {
                            CellData neighbor = MineField[nr, nc];
                            if (!neighbor.IsMine && !neighbor.IsRevealed && !visited.Contains(neighbor) && !neighbor.IsFlagged)
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

        // Unified mouse handler for double-click support
        private void HandleMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                if (CheckIfFlagsMatchMines(cell))
                {
                    RevealDoubleClickCells(cell);
                }
            }
        }

        private bool CheckIfFlagsMatchMines(CellData cell)
        {
            int currRow = cell.Row;
            int currCol = cell.Col;
            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int nr = currRow + dr;
                    int nc = currCol + dc;

                    if (nr >= 0 && nr < _rowCount && nc >= 0 && nc < _colCount)
                    {
                        if ((!MineField[nr, nc].IsMine && MineField[nr, nc].IsFlagged) ||
                            (MineField[nr, nc].IsMine && !MineField[nr, nc].IsFlagged))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void RevealDoubleClickCells(CellData startCell)
        {
            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = startCell.Row + dr;
                    int nc = startCell.Col + dc;

                    if (nr >= 0 && nr < _rowCount && nc >= 0 && nc < _colCount)
                    {
                        CellData neighbor = MineField[nr, nc];

                        if (!neighbor.IsMine && !neighbor.IsRevealed && !neighbor.IsFlagged)
                        {
                            neighbor.IsRevealed = true;
                            if (neighbor.AdjacentMines > 0)
                            {
                                Button btn = GetButtonForCell(neighbor);
                                btn.Content = neighbor.AdjacentMines.ToString();
                                btn.Foreground = GetMineCountColor(neighbor.AdjacentMines);
                                btn.Background = Brushes.White;
                                btn.FocusVisualStyle = null;
                                btn.IsEnabled = true;
                            }
                            else
                            {
                                RevealValidCells(neighbor);
                            }
                        }
                    }
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MineFieldGrid.Children.Clear();
            GenerateMinePositions();
            CreateBoard();
            _mineCount = MinePositions.Length;
            MineCounter.Text = _mineCount.ToString();
            Timer.Text = "0";
            StartTimer();
        }
    }
}

using MineSweeper.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MineSweeper.Services
{
    internal class UpdateUIHelper
    {
        private readonly Panel _mineFieldGrid;
        private readonly Dictionary<CellData, Button> _cellButtonMap = new();

        public UpdateUIHelper(Panel mineFieldGrid)
        {
            _mineFieldGrid = mineFieldGrid;
        }

        public void UpdateCellUI(CellData cell)
        {
            var btn = GetButtonForCell(cell);
            if (btn == null) return;

            if (cell.IsRevealed)
            {
                if (cell.IsMine)
                {
                    btn.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Assets/mine.png"))
                    };
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
                    btn.Content = "";
                    btn.Background = Brushes.White;
                    btn.IsEnabled = false;
                }
            }
            else if (cell.IsFlagged)
            {
                btn.Content = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/flag.png"))
                };
            }
            else
            {
                btn.Content = null;
                btn.Background = Brushes.LightGray;
                btn.IsEnabled = true;
            }
        }

        public void UpdateMultipleCells(CellData[,] cells)
        {
            foreach (var cell in cells)
            {
                UpdateCellUI(cell);
            }
        }

        public Button? GetButtonForCell(CellData cell)
        {
            _cellButtonMap.TryGetValue(cell, out var btn);
            return btn;
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

        public void RegisterCellButton(CellData cell, Button btn)
        {
            _cellButtonMap[cell] = btn;
        }
        public void Clear()
        {
            _cellButtonMap.Clear();
            _mineFieldGrid.Children.Clear();
        }
        public void UpdateTimer(TextBlock timer, int time)
        {
            timer.Text = time.ToString();
        }
    }
}

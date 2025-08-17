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

        public void UpdateCellUI(CellData cell, bool isExplodedMine = false, bool isWrongFlag = false, bool forceReveal = false)
        {
            var btn = GetButtonForCell(cell);
            if (btn == null) return;

            if (isWrongFlag)
            {
                btn.Content = new Grid
                {
                    Children =
                    {
                        new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Assets/flag.png")),
                            Stretch = Stretch.Uniform
                        },
                        new TextBlock
                        {
                            Text = "❌",
                            Foreground = Brushes.Red,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    }
                };
                btn.Background = Brushes.LightPink;
                btn.IsEnabled = false;
                return;
            }

            if (cell.IsRevealed || forceReveal)
            {
                if (cell.IsMine)
                {
                    if (isExplodedMine)
                    {
                        btn.Content = new Grid
                        {
                            Background = Brushes.Red,
                            Children =
                            {
                                new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/mine.png")),
                                    Stretch = Stretch.Uniform
                                }
                            }
                        };
                    }
                    else
                    {
                        btn.Content = new Grid
                        {
                            Children =
                            {
                                new Image
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/mine.png")),
                                    Stretch = Stretch.Uniform
                                }
                            }
                        };
                    }
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
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/flag.png")),
                    Stretch = Stretch.Uniform
                };
                btn.Background = Brushes.LightGray;
                btn.IsEnabled = true;
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
        public void RevealAll(CellData[,] mineField, CellData? explodedMine)
        {
            foreach (var cell in mineField)
            {
                if (cell == explodedMine)
                {
                    UpdateCellUI(cell, isExplodedMine: true);
                }
                else if (cell.IsMine && cell.IsFlagged)
                {
                    continue;
                }
                else if (cell.IsMine && !cell.IsFlagged)
                {
                    UpdateCellUI(cell, forceReveal: true);
                }
                else if (!cell.IsMine && cell.IsFlagged)
                {
                    UpdateCellUI(cell, isWrongFlag: true);
                }
                else if (!cell.IsRevealed)
                {
                    UpdateCellUI(cell, forceReveal: true);
                }
                var btn = GetButtonForCell(cell);
                if (btn != null)
                    btn.IsEnabled = false;
            }
        }

    }
}

using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MineSweeper.Services
{
    internal class UpdateUIHelper(Panel mineFieldGrid)
    {
        private readonly Panel _mineFieldGrid = mineFieldGrid;
        private readonly Dictionary<CellData, Button> _cellButtonMap = [];

        /// <summary>
        /// Updates the UI of a single cell based on its state.
        /// Handles mines, numbers, flags, wrong flags, and exploded mines.
        /// </summary>
        public void UpdateCellUI(CellData cell, bool isExplodedMine = false, bool isWrongFlag = false, bool forceReveal = false)
        {
            var btn = GetButtonForCell(cell);
            if (btn == null) return;

            if (isWrongFlag)
            {
                RenderWrongFlag(btn);
                return;
            }

            if (cell.IsRevealed || forceReveal)
            {
                if (cell.IsMine)
                {
                    if (isExplodedMine) RenderExplodedMine(btn);
                    else RenderMine(btn);
                }
                else if (cell.AdjacentMines > 0)
                {
                    RenderNumber(btn, cell.AdjacentMines);
                }
                else
                {
                    RenderEmpty(btn);
                }
            }
            else if (cell.IsFlagged)
            {
                RenderFlag(btn);
            }
            else
            {
                RenderHidden(btn);
            }
        }

        /// <summary>
        /// Updates the UI of multiple cells in bulk.
        /// </summary>
        public void UpdateMultipleCells(CellData[,] cells)
        {
            foreach (var cell in cells)
            {
                UpdateCellUI(cell);
            }
        }

        /// <summary>
        /// Gets the UI button mapped to a given cell.
        /// </summary>
        public Button? GetButtonForCell(CellData cell)
        {
            _cellButtonMap.TryGetValue(cell, out var btn);
            return btn;
        }

        /// <summary>
        /// Registers a mapping between a cell and its button for later UI updates.
        /// </summary>
        public void RegisterCellButton(CellData cell, Button btn)
        {
            _cellButtonMap[cell] = btn;
        }

        /// <summary>
        /// Clears the board by removing all registered buttons and resetting state.
        /// </summary>
        public void Clear()
        {
            _cellButtonMap.Clear();
            _mineFieldGrid.Children.Clear();
        }

        /// <summary>
        /// Updates the timer display in the UI.
        /// </summary>
        public void UpdateTimer(TextBlock timer, int time)
        {
            timer.Text = time.ToString();
        }

        /// <summary>
        /// Reveals the entire board after game over.
        /// Handles exploded mine, wrong flags, and correct mines properly.
        /// </summary>
        public void RevealAll(CellData[,] mineField, CellData? explodedMine)
        {
            foreach (var cell in mineField)
            {
                var btn = GetButtonForCell(cell);
                if (btn != null) btn.IsEnabled = false;
                if (cell == explodedMine)
                {
                    UpdateCellUI(cell, isExplodedMine: true);
                }
                else if (cell.IsMine && cell.IsFlagged)
                {
                    if (btn != null) btn.IsEnabled = false;
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
                if (btn != null) btn.IsEnabled = false;
            }
        }

        /// <summary>
        /// Renders a wrong flag marker (flag + ❌).
        /// </summary>
        private static void RenderWrongFlag(Button btn)
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
                    new Viewbox
                    {
                        Child = new TextBlock
                        {
                            Text = "❌",
                            Foreground = Brushes.Red,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    }
                }
            };
            btn.Background = Brushes.LightPink;
            btn.IsEnabled = false;
        }

        /// <summary>
        /// Renders the mine that caused the explosion (red background).
        /// </summary>
        private static void RenderExplodedMine(Button btn)
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

        /// <summary>
        /// Renders a normal mine.
        /// </summary>
        private static void RenderMine(Button btn)
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

        /// <summary>
        /// Renders a number representing adjacent mines.
        /// </summary>
        private static void RenderNumber(Button btn, int number)
        {
            btn.Content = number.ToString();
            btn.Foreground = GetMineCountColor(number);
            btn.Background = Brushes.White;
            btn.FocusVisualStyle = null;
            btn.IsEnabled = true;
        }

        /// <summary>
        /// Renders an empty revealed cell.
        /// </summary>
        private static void RenderEmpty(Button btn)
        {
            btn.Content = "";
            btn.Background = Brushes.White;
            btn.IsEnabled = false;
        }

        /// <summary>
        /// Renders a flagged cell.
        /// </summary>
        private static void RenderFlag(Button btn)
        {
            btn.Content = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Assets/flag.png")),
                Stretch = Stretch.Uniform
            };
            btn.Background = Brushes.LightGray;
            btn.IsEnabled = true;
        }

        /// <summary>
        /// Renders a hidden unrevealed cell.
        /// </summary>
        private static void RenderHidden(Button btn)
        {
            btn.Content = null;
            btn.Background = Brushes.LightGray;
            btn.IsEnabled = true;
        }

        /// <summary>
        /// Maps mine counts (1–8) to standard Minesweeper colors.
        /// </summary>
        private static SolidColorBrush GetMineCountColor(int count)
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
    }
}

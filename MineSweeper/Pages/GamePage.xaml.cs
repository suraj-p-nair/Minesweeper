using MineSweeper.Models;
using MineSweeper.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MineSweeper.Pages
{
    public partial class GamePage : Page
    {
        private readonly GameProperties _gameProperties;
        private readonly BoardService _gameBoard = new();
        private readonly GameTimerService _gameTimer = new();
        private readonly GameEngine _gameEngine = new();
        private readonly UpdateUIHelper _uiHelper;
        private readonly MineGenerator _mineGenerator = new();
        private int[] MinePositions = [];
        private int _mineCount;
        private CellData[,] MineField = new CellData[0, 0];

        public GamePage(GameProperties gameProperties)
        {
            _gameProperties = gameProperties;
            _mineCount = _gameProperties.MineCount;
            MinePositions = [.. _mineGenerator.GenerateMinePositions(_gameProperties)];

            InitializeComponent();

            MineFieldGrid.Rows = _gameProperties.RowCount;
            MineFieldGrid.Columns = _gameProperties.ColumnCount;

            _uiHelper = new UpdateUIHelper(MineFieldGrid);

            SetupBoard();
            _gameTimer.StartTimer(Timer);
        }

        private void SetupBoard()
        {
            _uiHelper.Clear();

            MineField = _gameBoard.CreateBoard(_gameProperties, MinePositions);

            _gameBoard.CalculateAdjacentMineCount(_gameProperties, MineField);

            foreach (var cell in MineField)
            {
                Button btn = new()
                {
                    Tag = cell,
                    FontFamily = new FontFamily("Segoe UI Emoji"),
                    FontSize = 12,
                    Background = Brushes.LightGray
                };

                btn.Click += RevealSingleCell;
                btn.MouseRightButtonDown += FlagCell;
                btn.MouseDoubleClick += HandleMouseClick;

                MineFieldGrid.Children.Add(btn);
                _uiHelper.RegisterCellButton(cell, btn);
            }
        }

        private void FlagCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                var result = _gameEngine.ToggleFlag(cell, _mineCount);

                cell.IsFlagged = result.isFlagged;
                _mineCount = result.mineCount;

                _uiHelper.UpdateCellUI(cell);
                MineCounter.Text = _mineCount.ToString();
            }
        }

        private void RevealSingleCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                var revealedCells = _gameEngine.RevealCell(cell, MineField);

                foreach (var revealed in revealedCells)
                {
                    _uiHelper.UpdateCellUI(revealed);
                }
            }
        }

        private void HandleMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                var revealedCells = _gameEngine.RevealNeighborsIfFlagsMatch(cell, MineField);

                foreach (var revealed in revealedCells)
                {
                    _uiHelper.UpdateCellUI(revealed);
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MinePositions = _mineGenerator.GenerateMinePositions(_gameProperties).ToArray();
            SetupBoard();
            _mineCount = MinePositions.Length;
            MineCounter.Text = _mineCount.ToString();
            Timer.Text = "0";
            _gameTimer.StartTimer(Timer);
        }
    }
}

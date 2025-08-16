using MineSweeper.Models;
using MineSweeper.Services;
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

            _gameTimer.OnTimeChanged += (time) =>
            {
                Dispatcher.Invoke(() => _uiHelper.UpdateTimer(Timer, time));
            };

            SetupBoard();
        }

        private void SetupBoard()
        {
            _uiHelper.Clear();

            MineField = _gameBoard.CreateBoard(_gameProperties, MinePositions);

            _gameBoard.CalculateAdjacentMineCount(_gameProperties, MineField);

            var (width, height) = BoardSizeFactory.GetSize(
                _gameProperties.RowCount,
                _gameProperties.ColumnCount);

            MineFieldContainer.Width = width;
            MineFieldContainer.Height = height;

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
                if (!_gameTimer.IsRunning)   // <-- we’ll expose this property
                {
                    _gameTimer.StartTimer();
                }
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
                if (!_gameTimer.IsRunning)   // <-- we’ll expose this property
                {
                    _gameTimer.StartTimer();
                }
                if (cell.IsMine)
                {
                    _gameTimer.StopTimer();
                    _uiHelper.RevealAll(MineField, cell);
                    EndGame(false);
                    return;
                }
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
                if (!cell.IsRevealed || cell.AdjacentMines == 0)
                    return;
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
            _gameTimer.StopTimer();
            _gameTimer.ResetTimer();
        }

        private void EndGame(bool isWin)
        {
            int currentTime = _gameTimer.time;
            int? bestTime = SaveManager.GetBestTime(_gameProperties.Difficulty);
            string title = isWin ? "You Win!" : "Game Over";

            var overlay = new ResultOverlay(title, currentTime, bestTime);
            OverlayHost.Content = overlay;
            OverlayHost.Visibility = Visibility.Visible;
        }


    }
}

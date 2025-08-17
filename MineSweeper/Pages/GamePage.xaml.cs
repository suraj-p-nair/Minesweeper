using MineSweeper.Models;
using MineSweeper.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MineSweeper.Pages
{
    /// <summary>
    /// GamePage represents the main gameplay surface.
    /// - Initializes board, timer, and engine
    /// - Handles user input (left/right/double click)
    /// - Updates UI and game state through helper services
    /// - Triggers win/lose overlays
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly GameProperties _gameProperties;
        private readonly GameTimerService _gameTimer = new();
        private readonly UpdateUIHelper _uiHelper;

        private int[] MinePositions = [];
        private int _mineCount;
        private CellData[,] MineField = new CellData[0, 0];

        public GamePage(GameProperties gameProperties)
        {
            _gameProperties = gameProperties;
            _mineCount = _gameProperties.MineCount;

            // Generate random mine layout
            MinePositions = [.. MineGenerator.GenerateMinePositions(_gameProperties)];

            InitializeComponent();

            // Configure grid
            MineFieldGrid.Rows = _gameProperties.RowCount;
            MineFieldGrid.Columns = _gameProperties.ColumnCount;

            _uiHelper = new UpdateUIHelper(MineFieldGrid);

            // Hook timer -> update UI on tick
            _gameTimer.OnTimeChanged += (time) =>
            {
                Dispatcher.Invoke(() => _uiHelper.UpdateTimer(Timer, time));
            };

            SetupBoard();
        }

        /// <summary>
        /// Creates the logical minefield + UI grid of buttons.
        /// </summary>
        private void SetupBoard()
        {
            _uiHelper.Clear();

            MineField = BoardService.CreateBoard(_gameProperties, MinePositions);
            BoardService.CalculateAdjacentMineCount(_gameProperties, MineField);

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

                // Attach input handlers
                btn.Click += RevealSingleCell;                // left click
                btn.MouseRightButtonDown += FlagCell;         // right click
                btn.MouseDoubleClick += HandleMouseClick;     // double click

                MineFieldGrid.Children.Add(btn);
                _uiHelper.RegisterCellButton(cell, btn);
                MineCounter.Text = _mineCount.ToString();
            }
        }

        /// <summary>
        /// Handles flagging/unflagging cells with right-click.
        /// </summary>
        private void FlagCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                if (!_gameTimer.IsRunning)
                {
                    _gameTimer.StartTimer();
                }

                var result = GameEngine.ToggleFlag(cell, _mineCount);

                cell.IsFlagged = result.isFlagged;
                _mineCount = result.mineCount;

                _uiHelper.UpdateCellUI(cell);
                MineCounter.Text = _mineCount.ToString();
            }
        }

        /// <summary>
        /// Handles left-click reveal on a single cell.
        /// </summary>
        private void RevealSingleCell(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                if (cell.IsRevealed) return;
                if (!_gameTimer.IsRunning)
                {
                    _gameTimer.StartTimer();
                }

                // Clicked a mine -> lose
                if (cell.IsMine)
                {
                    _gameTimer.StopTimer();
                    cell.IsRevealed = true;
                    _uiHelper.RevealAll(MineField, cell); // marks this mine red
                    EndGame(false);
                    return;
                }

                // Normal reveal
                var revealedCells = GameEngine.RevealCell(cell, MineField);

                foreach (var revealed in revealedCells)
                {
                    _uiHelper.UpdateCellUI(revealed);
                }

                // Check win condition
                if (GameEngine.HasWon(MineField, _gameProperties.MineCount))
                {
                    _gameTimer.StopTimer();
                    SaveManager.UpdateBestTime(_gameProperties.Difficulty, _gameTimer.time);
                    EndGame(true);
                }
            }
        }

        /// <summary>
        /// Handles double-click on revealed cell (chording).
        /// Reveals neighbors if flag count matches.
        /// </summary>
        private void HandleMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CellData cell)
            {
                if (!cell.IsRevealed || cell.AdjacentMines == 0)
                    return;

                var (revealedCells, explodedMine) = GameEngine.RevealNeighborsIfFlagsMatch(cell, MineField);

                foreach (var revealed in revealedCells)
                {
                    _uiHelper.UpdateCellUI(revealed);
                }

                if (explodedMine != null)
                {
                    // Mine exploded from wrong flag placement
                    explodedMine.IsRevealed = true;
                    _gameTimer.StopTimer();
                    _uiHelper.RevealAll(MineField, explodedMine);
                    EndGame(false);
                }
                else if (GameEngine.HasWon(MineField, _gameProperties.MineCount))
                {
                    _gameTimer.StopTimer();
                    SaveManager.UpdateBestTime(_gameProperties.Difficulty, _gameTimer.time);
                    EndGame(true);
                }
            }
        }

        /// <summary>
        /// Resets the game to a fresh board.
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MinePositions = [.. MineGenerator.GenerateMinePositions(_gameProperties)];
            SetupBoard();
            _mineCount = MinePositions.Length;
            MineCounter.Text = _mineCount.ToString();

            Timer.Text = "0";
            _gameTimer.StopTimer();
            _gameTimer.ResetTimer();

            // Ensure overlay disappears on reset
            OverlayHost.Content = null;
            OverlayHost.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Ends the game (win/lose) and shows result overlay.
        /// </summary>
        private void EndGame(bool isWin)
        {
            int currentTime = _gameTimer.time;
            int? bestTime = SaveManager.GetBestTime(_gameProperties.Difficulty);
            string title = isWin ? "You Win!" : "Game Over";

            var overlay = new ResultOverlay(title, currentTime, bestTime);
            overlay.GoToMainMenuRequested += () =>
            {
                NavigationService?.Navigate(new StartPage());
            };

            OverlayHost.Content = overlay;
            OverlayHost.Visibility = Visibility.Visible;
        }
    }
}

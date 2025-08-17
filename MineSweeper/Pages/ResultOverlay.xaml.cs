using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.Pages
{
    public partial class ResultOverlay : UserControl
    {
        public event Action? GoToMainMenuRequested;
        public ResultOverlay(string title, int currentTime, int? bestTime)
        {
            InitializeComponent();
            TitleText.Text = title;
            CurrentTimeText.Text = $"Current Time: {currentTime}s";
            BestTimeText.Text = $"Best Time: {(bestTime.HasValue ? bestTime.Value + "s" : "0s")}";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed; // just hide overlay
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            GoToMainMenuRequested?.Invoke();
        }
    }
}

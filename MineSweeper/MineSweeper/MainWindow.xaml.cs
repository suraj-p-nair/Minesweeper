using System.Windows;
using System.Windows.Controls;

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateBoard();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < 100; i++)
            {
                Button btn = new()
                {
                    Content = $"{i}"
                };
                MineField.Children.Add(btn);
            }
        }
    }
}

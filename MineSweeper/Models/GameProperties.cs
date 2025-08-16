namespace MineSweeper.Models
{
    public class GameProperties
    {
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int MineCount { get; set; }
        public Difficulty Difficulty { get; set; }
        public string GameMode => Difficulty.ToString();
        public GameProperties(Difficulty difficulty)
        {
            Difficulty = difficulty;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    RowCount = 9;
                    ColumnCount = 9;
                    MineCount = 10;
                    break;
                case Difficulty.Medium:
                    RowCount = 16;
                    ColumnCount = 16;
                    MineCount = 40;
                    break;
                case Difficulty.Hard:
                    RowCount = 16;
                    ColumnCount = 30;
                    MineCount = 60;
                    break;
            }
        }
    }
}

namespace MineSweeper.Models
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public class BestTimes
    {
        // Store best seconds for each difficulty
        public Dictionary<Difficulty, int> Times { get; set; } = new();
    }
}


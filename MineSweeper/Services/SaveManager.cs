using System.IO;
using System.Text.Json;
using MineSweeper.Models;

namespace MineSweeper.Services
{
    public static class SaveManager
    {
        private static readonly string SaveFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "MineSweeper", "best_times.json");

        private static BestTimes _bestTimes;

        public static void Load()
        {
            if (!File.Exists(SaveFilePath))
            {
                _bestTimes = new BestTimes();
                return;
            }

            string json = File.ReadAllText(SaveFilePath);
            _bestTimes = JsonSerializer.Deserialize<BestTimes>(json)
                         ?? new BestTimes();
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath)!);
            string json = JsonSerializer.Serialize(_bestTimes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        public static int? GetBestTime(Difficulty difficulty)
        {
            return _bestTimes.Times.TryGetValue(difficulty, out var seconds) ? seconds : null;
        }

        public static void UpdateBestTime(Difficulty difficulty, int seconds)
        {
            if (!_bestTimes.Times.TryGetValue(difficulty, out var current) || seconds < current)
            {
                _bestTimes.Times[difficulty] = seconds;
                Save();
            }
        }
    }
}

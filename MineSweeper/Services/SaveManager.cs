using System.IO;
using System.Text.Json;
using MineSweeper.Models;

namespace MineSweeper.Services
{
    /// <summary>
    /// Manages saving and loading of best times for different difficulty levels.
    /// Uses JSON serialization to persist times to disk.
    /// </summary>
    public static class SaveManager
    {
        private static readonly string SaveFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "MineSweeper", "best_times.json");

        private static BestTimes _bestTimes = new();

        /// <summary>
        /// Loads best times from disk.
        /// If no save file exists, initializes a new empty record.
        /// </summary>
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

        /// <summary>
        /// Saves the current best times to disk.
        /// </summary>
        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath)!);
            string json = JsonSerializer.Serialize(_bestTimes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        /// <summary>
        /// Gets the best recorded time for the given difficulty, if available.
        /// </summary>
        public static int? GetBestTime(Difficulty difficulty)
        {
            return _bestTimes.Times.TryGetValue(difficulty, out var seconds) ? seconds : null;
        }

        /// <summary>
        /// Updates the best time for a difficulty if the new time is lower.
        /// Persists the updated times to disk immediately.
        /// </summary>
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

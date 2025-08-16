using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{
    public class GameConfig
    {
        public static GameProperties Easy { get; set; } = new GameProperties(Difficulty.Easy);
        public static GameProperties Medium { get; set; } = new GameProperties(Difficulty.Medium);  
        public static GameProperties Hard { get; set; } = new GameProperties(Difficulty.Hard);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{
    public class GameProperties
    {
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int MineCount { get; set; }

        public GameProperties(int rowCount, int columnCount, int mineCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            MineCount = mineCount;
        }
        public GameProperties(Difficulty difficulty)
        {
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
        public GameProperties() { }
    }
}


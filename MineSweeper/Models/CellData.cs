using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Models
{
    public class CellData
    {
        public bool IsRevealed { get; set; } = false;
        public bool IsMine { get; set; } = false;
        public int AdjacentMines { get; set; } = 0;
        public string DisplayValue { get; set; } = "";
        public bool IsFlagged { get; set; } = false;
        public int Row { get; set; }
        public int Col { get; set; }
    }

}

using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Services
{
    public class MineGenerator
    {
        public List<int> GenerateMinePositions(GameProperties properties)
        {
            Random rng = new Random();
            return [.. Enumerable.Range(0, properties.RowCount * properties.ColumnCount)
                                        .OrderBy(_ => rng.Next())
                                        .Take(properties.MineCount)];
        }
    }
}

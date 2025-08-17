using MineSweeper.Models;
using System.Linq;

namespace MineSweeper.Services
{
    /// <summary>
    /// Builds the logical minefield grid and computes adjacency counts.
    /// </summary>
    public class BoardService
    {
        /// <summary>
        /// Creates a minefield for the given properties and mine indices.
        /// </summary>
        public static CellData[,] CreateBoard(GameProperties properties, int[] minePositions)
        {
            int rowCount = properties.RowCount;
            int colCount = properties.ColumnCount;
            var mineField = new CellData[rowCount, colCount];

            for (int i = 0; i < rowCount * colCount; i++)
            {
                var cellData = new CellData
                {
                    IsMine = minePositions.Contains(i),
                    Row = i / colCount,
                    Col = i % colCount,
                    AdjacentMines = 0
                };

                mineField[cellData.Row, cellData.Col] = cellData;
            }

            return mineField;
        }

        /// <summary>
        /// Computes AdjacentMines for each non-mine cell.
        /// </summary>
        public static void CalculateAdjacentMineCount(GameProperties properties, CellData[,] mineField)
        {
            int rowCount = properties.RowCount;
            int colCount = properties.ColumnCount;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (mineField[r, c].IsMine)
                    {
                        UpdateAdjacentCells(mineField[r, c], properties, mineField);
                    }
                }
            }
        }

        /// <summary>
        /// Increments adjacency counters for neighbors around a single mine cell.
        /// </summary>
        private static void UpdateAdjacentCells(CellData currentCell, GameProperties properties, CellData[,] mineField)
        {
            int rowCount = properties.RowCount;
            int colCount = properties.ColumnCount;

            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = currentCell.Row + dr;
                    int nc = currentCell.Col + dc;

                    if (nr >= 0 && nr < rowCount && nc >= 0 && nc < colCount)
                    {
                        if (!mineField[nr, nc].IsMine)
                        {
                            mineField[nr, nc].AdjacentMines++;
                        }
                    }
                }
            }
        }
    }
}

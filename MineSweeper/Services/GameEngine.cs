using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MineSweeper.Services
{
    public class GameEngine
    {

       
        public (bool isFlagged, int mineCount) ToggleFlag(CellData cell, int mineCount)
        {
            if (cell.IsRevealed)
                return (cell.IsFlagged, mineCount);

            if (!cell.IsFlagged && mineCount == 0)
                return (cell.IsFlagged, mineCount);

            cell.IsFlagged = !cell.IsFlagged;
            mineCount += cell.IsFlagged ? -1 : 1;

            return (cell.IsFlagged, mineCount);
        }
    


       
        public List<CellData> RevealCell(CellData startCell, CellData[,] mineField)
        {
            var revealedCells = new List<CellData>();

            if (startCell.IsRevealed || startCell.IsFlagged)
                return revealedCells;

            if (startCell.IsMine)
            {
                startCell.IsRevealed = true;
                revealedCells.Add(startCell);
                return revealedCells;
            }

            Queue<CellData> queue = new();
            HashSet<CellData> visited = new();

            queue.Enqueue(startCell);
            visited.Add(startCell);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (cell.IsRevealed) continue;

                cell.IsRevealed = true;
                revealedCells.Add(cell);

                if (cell.AdjacentMines == 0)
                {
                    foreach (var neighbor in GetNeighbors(cell, mineField))
                    {
                        if (!neighbor.IsRevealed && !neighbor.IsFlagged && !visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return revealedCells;
        }

        private IEnumerable<CellData> GetNeighbors(CellData cell, CellData[,] mineField)
        {
            int rowCount = mineField.GetLength(0);
            int colCount = mineField.GetLength(1);

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = cell.Row + dr;
                    int nc = cell.Col + dc;

                    if (nr >= 0 && nr < rowCount && nc >= 0 && nc < colCount)
                        yield return mineField[nr, nc];
                }
            }
        }

        public List<CellData> RevealNeighborsIfFlagsMatch(CellData startCell, CellData[,] mineField)
        {
            if (!CheckIfFlagsMatch(startCell, mineField))
                return new List<CellData>();

            var revealed = new List<CellData>();

            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = startCell.Row + dr;
                    int nc = startCell.Col + dc;

                    if (nr >= 0 && nr < mineField.GetLength(0) && nc >= 0 && nc < mineField.GetLength(1))
                    {
                        CellData neighbor = mineField[nr, nc];

                        if (!neighbor.IsMine && !neighbor.IsRevealed && !neighbor.IsFlagged)
                        {
                            // delegate to RevealCell (so floodfill reuse happens here)
                            var revealedFromNeighbor = RevealCell(neighbor, mineField);
                            revealed.AddRange(revealedFromNeighbor);
                        }
                    }
                }
            }

            return revealed;
        }

        private bool CheckIfFlagsMatch(CellData cell, CellData[,] mineField)
        {
            int currRow = cell.Row;
            int currCol = cell.Col;

            foreach (int dr in new[] { -1, 0, 1 })
            {
                foreach (int dc in new[] { -1, 0, 1 })
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int nr = currRow + dr;
                    int nc = currCol + dc;

                    if (nr >= 0 && nr < mineField.GetLength(0) && nc >= 0 && nc < mineField.GetLength(1))
                    {
                        if ((!mineField[nr, nc].IsMine && mineField[nr, nc].IsFlagged) ||
                            (mineField[nr, nc].IsMine && !mineField[nr, nc].IsFlagged))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


    }
}

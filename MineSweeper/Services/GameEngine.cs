using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MineSweeper.Services
{
    /// <summary>
    /// Encapsulates gameplay operations:
    /// - Flag toggling (and mine counter accounting)
    /// - Flood-reveal for empty cells
    /// - Chording (reveal neighbors when flags match)
    /// - Win condition check
    /// </summary>
    public class GameEngine
    {
        /// <summary>
        /// Toggle a flag on a cell and adjust the remaining mine counter.
        /// Returns the new flag state and updated mine count.
        /// </summary>
        public static (bool isFlagged, int mineCount) ToggleFlag(CellData cell, int mineCount)
        {
            if (cell.IsRevealed)
                return (cell.IsFlagged, mineCount);

            if (!cell.IsFlagged && mineCount == 0)
                return (cell.IsFlagged, mineCount);

            cell.IsFlagged = !cell.IsFlagged;
            mineCount += cell.IsFlagged ? -1 : 1;

            return (cell.IsFlagged, mineCount);
        }

        /// <summary>
        /// Reveal starting cell; flood-fill neighbors if AdjacentMines == 0.
        /// Returns the list of cells that became revealed by this action.
        /// </summary>
        public static List<CellData> RevealCell(CellData startCell, CellData[,] mineField)
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

        /// <summary>
        /// Enumerates all valid neighbors around a given cell.
        /// </summary>
        private static IEnumerable<CellData> GetNeighbors(CellData cell, CellData[,] mineField)
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

        /// <summary>
        /// If the number of flags around a revealed numbered cell equals its AdjacentMines,
        /// reveal all unflagged neighbors. Returns (revealedCells, explodedMine).
        /// explodedMine is non-null if a mine was revealed due to a wrong flag configuration.
        /// </summary>
        public static (List<CellData> revealedCells, CellData? explodedMine) RevealNeighborsIfFlagsMatch(CellData cell, CellData[,] mineField)
        {
            var revealed = new List<CellData>();

            if (!cell.IsRevealed || cell.AdjacentMines == 0)
                return (revealed, null);

            int flaggedCount = 0;
            var neighbors = GetNeighbors(cell, mineField);

            foreach (var n in neighbors)
            {
                if (n.IsFlagged) flaggedCount++;
            }

            if (flaggedCount != cell.AdjacentMines)
            {
                return (revealed, null);
            }

            foreach (var n in neighbors)
            {
                if (!n.IsFlagged && !n.IsRevealed)
                {
                    if (n.IsMine)
                    {
                        n.IsRevealed = true;
                        return (revealed, n);
                    }

                    revealed.AddRange(RevealCell(n, mineField));
                }
            }

            return (revealed, null);
        }

        /// <summary>
        /// Returns true when all non-mine cells are revealed.
        /// </summary>
        public static bool HasWon(CellData[,] mineField, int totalMines)
        {
            int totalCells = mineField.Length;
            int safeCells = totalCells - totalMines;
            int revealedSafe = mineField.Cast<CellData>().Count(c => !c.IsMine && c.IsRevealed);

            return revealedSafe == safeCells;
        }
    }
}

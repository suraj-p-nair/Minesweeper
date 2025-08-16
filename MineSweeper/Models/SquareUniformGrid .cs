using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.Models
{
    public class SquareUniformGrid : Panel
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Rows <= 0 || Columns <= 0)
                return new Size(0, 0);

            double cellSize = Math.Min(availableSize.Width / Columns, availableSize.Height / Rows);

            foreach (UIElement child in InternalChildren)
                child.Measure(new Size(cellSize, cellSize));

            return new Size(cellSize * Columns, cellSize * Rows);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Rows <= 0 || Columns <= 0)
                return finalSize;

            double cellSize = Math.Min(finalSize.Width / Columns, finalSize.Height / Rows);

            double totalWidth = cellSize * Columns;
            double totalHeight = cellSize * Rows;

            double offsetX = (finalSize.Width - totalWidth) / 2;
            double offsetY = (finalSize.Height - totalHeight) / 2;

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                int row = i / Columns;
                int col = i % Columns;

                Rect rect = new Rect(
                    offsetX + col * cellSize,
                    offsetY + row * cellSize,
                    cellSize,
                    cellSize);

                InternalChildren[i].Arrange(rect);
            }

            return finalSize;
        }


    }

}

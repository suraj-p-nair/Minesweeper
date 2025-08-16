namespace MineSweeper.Services
{
    public static class BoardSizeFactory
    {
        public static (double width, double height) GetSize(int rows, int cols, double baseSize = 600)
        {
            double aspect = (double)cols / rows;

            if (aspect >= 1)
            {
                return (baseSize, baseSize / aspect);
            }
            else
            {
                return (baseSize * aspect, baseSize);
            }
        }
    }
}

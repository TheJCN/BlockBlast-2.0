namespace BlockBlast
{
    public class Board
    {
        public int[,] Cells { get; }
        public int Size { get; }

        public Board(int size)
        {
            Size = size;
            Cells = new int[size, size];
        }

        public bool CanPlaceFigure(int[,] shape, int startX, int startY)
        {
            for (var x = 0; x < shape.GetLength(0); x++)
            for (var y = 0; y < shape.GetLength(1); y++)
                if (shape[x, y] == 1)
                {
                    var cellX = startX + x;
                    var cellY = startY + y;
                    if (cellX >= Size || cellY >= Size || Cells[cellX, cellY] != 0)
                        return false;
                }
            return true;
        }

        public int ClearFullLines()
        {
            var linesCleared = 0;

            for (var y = 0; y < Size; y++)
            {
                var isFullLine = true;
                for (var x = 0; x < Size; x++)
                    if (Cells[x, y] == 0)
                    {
                        isFullLine = false;
                        break;
                    }

                if (isFullLine)
                {
                    for (var x = 0; x < Size; x++)
                        Cells[x, y] = 0;
                    linesCleared++;
                }
            }

            for (var x = 0; x < Size; x++)
            {
                var isFullLine = true;
                for (var y = 0; y < Size; y++)
                    if (Cells[x, y] == 0)
                    {
                        isFullLine = false;
                        break;
                    }

                if (isFullLine)
                {
                    for (var y = 0; y < Size; y++)
                        Cells[x, y] = 0;
                    linesCleared++;
                }
            }

            return linesCleared;
        }
    }
}
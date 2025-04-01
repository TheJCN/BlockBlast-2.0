namespace BlockBlast
{
    public class GameModel
    {
        public Board Board { get; }
        public List<Figure> AvailableFigures { get; }
        public List<Figure> PlacedFigures { get; }
        public int Score { get; private set; }

        public GameModel(int size)
        {
            Board = new Board(size);
            AvailableFigures = new List<Figure>();
            PlacedFigures = new List<Figure>();
            Score = 0;
            GenerateFigures();
        }

        public void GenerateFigures()
        {
            AvailableFigures.Clear();
            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                int figureType = random.Next(0, 6);
                AvailableFigures.Add(figureType switch
                {
                    0 => new Figure(new int[,] { { 1, 1 }, { 1, 1 } }),
                    1 => new Figure(new int[,] { { 1, 1, 1, 1 } }),
                    2 => new Figure(new int[,] { { 1, 0 }, { 1, 0 }, { 1, 1 } }),
                    3 => new Figure(new int[,] { { 1, 0 }, { 1, 1 }, { 1, 0 } }),
                    4 => new Figure(new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }),
                    _ => new Figure(new int[,] { { 1, 1, 1 }, { 0, 1, 0 } })
                });
            }
        }

        public bool PlaceFigure(Figure figure, int x, int y)
        {
            if (Board.CanPlaceFigure(figure.Shape, x, y))
            {
                for (var i = 0; i < figure.Shape.GetLength(0); i++)
                {
                    for (var j = 0; j < figure.Shape.GetLength(1); j++)
                    {
                        if (figure.Shape[i, j] == 1)
                        {
                            Board.Cells[x + i, y + j] = 1;
                        }
                    }
                }

                PlacedFigures.Add(figure);
                AvailableFigures.Remove(figure);

                var linesCleared = Board.ClearFullLines();
                if (linesCleared > 0)
                {
                    Score += linesCleared * 100;

                    if (linesCleared >= 2)
                    {
                        Score += 50;
                    }
                }

                if (AvailableFigures.Count == 0)
                {
                    GenerateFigures();
                }

                return true;
            }

            return false;
        }

        public bool CanPlaceFigure(Figure figure, int x, int y) => Board.CanPlaceFigure(figure.Shape, x, y);

        public bool IsGameOver()
        {
            foreach (var figure in AvailableFigures)
            {
                for (int x = 0; x < Board.Size; x++)
                {
                    for (int y = 0; y < Board.Size; y++)
                    {
                        if (Board.CanPlaceFigure(figure.Shape, x, y))
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

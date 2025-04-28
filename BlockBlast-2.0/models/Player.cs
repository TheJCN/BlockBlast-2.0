namespace BlockBlast_2._0.models;

public class Player
{
    public List<Figure> Figures { get; private set; }
    public int Score { get; private set; }
    public int Color { get; }

    public Player(int color)
    {
        Color = color;
        Figures = new List<Figure>();
        GenerateFigures();
    }

    public void GenerateFigures()
    {
        Figures.Clear();
        for (var i = 0; i < 3; i++)
        {
            Figures.Add(FigureFactory.CreateRandomFigure(Color));
        }
    }

    public void RemoveFigure(Figure figure)
    {
        Figures.Remove(figure);
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}
namespace BlockBlast_2._0.model;

public class Player
{
    public List<Figure> Figures { get; }
    public int Score { get; set; }

    public Player(int color, int score)
    {
        Score = score;
        Figures = [];
        GenerateFigures(color);
    }

    public void GenerateFigures(int color)
    {
        Figures.Clear();
        for (var i = 0; i < 3; i++)
            Figures.Add(FigureFactory.CreateRandomFigure(color));
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
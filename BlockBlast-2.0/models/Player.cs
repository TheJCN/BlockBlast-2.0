using System.Collections.Generic;
using System.Drawing;

namespace BlockBlast_2._0.model
{
    public class Player
    {
        public List<Figure> Figures { get; }
        public int Score { get; set; }
        public int Color { get; }

        public Player(int color, int score)
        {
            Color = color;
            Score = score;
            Figures = new List<Figure>();
            GenerateFigures();
        }

        public void GenerateFigures()
        {
            Figures.Clear();
            for (var i = 0; i < 3; i++)
                Figures.Add(FigureFactory.CreateRandomFigure(Color));
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
}
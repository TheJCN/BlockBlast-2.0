using BlockBlast_2._0.models;

namespace BlockBlast_2._0.utils;

public static class FigureFactory
{
    private static readonly Random Random = new();

    public static Figure CreateRandomFigure(int color)
    {
        var figureTypes = new List<Pixel[]>
        {
            // Квадрат 2x2
            new[] { new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 0, 1), new Pixel(color, 1, 1) },
            // Линия вертикальная
            new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, 0, 2), new Pixel(color, 0, 3) },
            // Линия горизонтальная
            new[] { new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 2, 0), new Pixel(color, 3, 0) },
            // L-образная
            new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, 0, 2), new Pixel(color, 1, 2) }
        };

        var index = Random.Next(figureTypes.Count);
        return new Figure(figureTypes[index]);
    }
}
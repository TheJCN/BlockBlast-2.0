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

        // Линии
        new[] { new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 2, 0), new Pixel(color, 3, 0) }, // Горизонталь
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, 0, 2), new Pixel(color, 0, 3) }, // Вертикаль

        // L-образные (4 поворота)
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, 0, 2), new Pixel(color, 1, 2) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 2, 0), new Pixel(color, 2, -1) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, -1), new Pixel(color, 0, -2), new Pixel(color, -1, -2) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, -1, 0), new Pixel(color, -2, 0), new Pixel(color, -2, 1) },

        // T-образные (4 поворота)
        new[] { new Pixel(color, 0, 0), new Pixel(color, -1, 1), new Pixel(color, 0, 1), new Pixel(color, 1, 1) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, -1), new Pixel(color, 1, 0), new Pixel(color, 0, 1) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, -1, 0), new Pixel(color, 0, -1), new Pixel(color, 1, 0) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, -1, 0), new Pixel(color, 0, 1), new Pixel(color, 0, -1) },

        // Z-образные (2 поворота)
        new[] { new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 1, 1), new Pixel(color, 2, 1) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, -1, 1), new Pixel(color, -1, 2) },

        // S-образные (2 поворота)
        new[] { new Pixel(color, 0, 0), new Pixel(color, -1, 0), new Pixel(color, -1, 1), new Pixel(color, -2, 1) },
        new[] { new Pixel(color, 0, 0), new Pixel(color, 0, 1), new Pixel(color, 1, 1), new Pixel(color, 1, 2) },

        // Квадрат 3x3 (9 блоков)
        new[]
        {
            new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 2, 0),
            new Pixel(color, 0, 1), new Pixel(color, 1, 1), new Pixel(color, 2, 1),
            new Pixel(color, 0, 2), new Pixel(color, 1, 2), new Pixel(color, 2, 2)
        },

        // Прямоугольник 2x3 (вертикальный)
        new[]
        {
            new Pixel(color, 0, 0), new Pixel(color, 1, 0),
            new Pixel(color, 0, 1), new Pixel(color, 1, 1),
            new Pixel(color, 0, 2), new Pixel(color, 1, 2)
        },

        // Прямоугольник 3x2 (горизонтальный)
        new[]
        {
            new Pixel(color, 0, 0), new Pixel(color, 1, 0), new Pixel(color, 2, 0),
            new Pixel(color, 0, 1), new Pixel(color, 1, 1), new Pixel(color, 2, 1)
        },
    };

    var index = Random.Next(figureTypes.Count);
    return new Figure(figureTypes[index]);
    }
}
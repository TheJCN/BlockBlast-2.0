namespace BlockBlast_2._0.Models;

public class Pixel(int color, int x, int y)
{
    public int Color { get; } = color;
    public int X { get; } = x;
    public int Y { get; } = y;
}
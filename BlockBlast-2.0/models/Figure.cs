namespace BlockBlast_2._0.model;

public class Figure
{
    public Pixel[] Pixels { get; }

    public Figure(Pixel[] pixels)
    {
        Pixels = pixels;
    }
        
    public Size GetSize()
    {
        var maxX = Pixels.Max(p => p.X);
        var maxY = Pixels.Max(p => p.Y);
        return new Size(maxX + 1, maxY + 1);
    }
}
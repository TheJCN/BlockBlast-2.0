using BlockBlast_2._0.Models;

namespace BlockBlast_Test;

[TestFixture]
public class FigureTests
{
    [Test]
    public void Figure_Constructor_SetsPixelsCorrectly()
    {
        var pixels = new Pixel[] 
        {
            new(0xFF0000, 0, 0),
            new(0xFF0000, 1, 0),
            new(0xFF0000, 0, 1)
        };
        
        var figure = new Figure(pixels);
        
        Assert.That(figure.Pixels, Has.Length.EqualTo(3));
        Assert.That(figure.Pixels, Is.EqualTo(pixels));
    }
}

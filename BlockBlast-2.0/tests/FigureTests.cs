using BlockBlast_2._0.models;
using NUnit.Framework;

namespace BlockBlast_2._0.tests;

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
        
        Assert.AreEqual(3, figure.Pixels.Length);
        Assert.AreEqual(pixels, figure.Pixels);
    }
}

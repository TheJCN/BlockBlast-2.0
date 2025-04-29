
using BlockBlast_2._0.models;
using NUnit.Framework;

namespace BlockBlast_2._0.tests;

[TestFixture]
public class PixelTests
{
    [Test]
    public void Pixel_Constructor_SetsPropertiesCorrectly()
    {
        const int color = 0xFF0000;
        const int x = 5;
        const int y = 10;
            
        var pixel = new Pixel(color, x, y);
            
        
        Assert.AreEqual(color, pixel.Color);
        Assert.AreEqual(x, pixel.X);
        Assert.AreEqual(y, pixel.Y);
    }
}
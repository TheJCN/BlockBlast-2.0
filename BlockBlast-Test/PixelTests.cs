using BlockBlast_2._0.models;

namespace BlockBlast_Test;

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
        Assert.Multiple(() =>
        {
            Assert.That(pixel.Color, Is.EqualTo(color));
            Assert.That(pixel.X, Is.EqualTo(x));
            Assert.That(pixel.Y, Is.EqualTo(y));
        });
    }
}
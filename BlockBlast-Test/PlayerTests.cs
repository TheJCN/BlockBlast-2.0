using BlockBlast_2._0.models;

namespace BlockBlast_Test;

[TestFixture]
public class PlayerTests
{
    [Test]
    public void Player_Constructor_SetsPropertiesCorrectly()
    {
        var color = 0xFF0000;
            
        var player = new Player(color);
        Assert.Multiple(() =>
        {
            Assert.That(player.Color, Is.EqualTo(color));
            Assert.That(player.Score, Is.EqualTo(0));
            Assert.That(player.Figures, Has.Count.EqualTo(3));
            Assert.That(player.Figures, Is.Not.Null);
        });
    }

    [Test]
    public void GenerateFigures_CreatesThreeFigures()
    {
        const int color = 0xFF0000;
        var player = new Player(color);
            
        player.GenerateFigures();
            
        Assert.That(player.Figures, Has.Count.EqualTo(3));
        Assert.That(player.Figures.All(f => f.Pixels.All(p => p.Color == color)), Is.True);
    }

    [Test]
    public void RemoveFigure_RemovesFigureFromCollection()
    {
        const int color = 0xFF0000;
        var player = new Player(color);
        var figureToRemove = player.Figures.First();
            
        player.RemoveFigure(figureToRemove);
            
        Assert.That(player.Figures, Has.Count.EqualTo(2));
        Assert.That(player.Figures, Does.Not.Contain(figureToRemove));
    }

    [Test]
    public void AddScore_IncreasesPlayerScore()
    {
        const int color = 0xFF0000;
        const int scoreToAdd = 100;
        
        var player = new Player(color);
        var initialScore = player.Score;
            
        player.AddScore(scoreToAdd);
            
        Assert.That(player.Score, Is.EqualTo(initialScore + scoreToAdd));
    }
}
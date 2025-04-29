using NUnit.Framework;
using BlockBlast_2._0.models;
using System.Linq;

namespace BlockBlast_2._0.tests;

[TestFixture]
public class PlayerTests
{
    [Test]
    public void Player_Constructor_SetsPropertiesCorrectly()
    {
        var color = 0xFF0000;
            
        var player = new Player(color);
            
        Assert.AreEqual(color, player.Color);
        Assert.AreEqual(0, player.Score);
        Assert.IsNotNull(player.Figures);
        Assert.AreEqual(3, player.Figures.Count);
    }

    [Test]
    public void GenerateFigures_CreatesThreeFigures()
    {
        var color = 0xFF0000;
        var player = new Player(color);
            
        player.GenerateFigures();
            
        Assert.AreEqual(3, player.Figures.Count);
        Assert.IsTrue(player.Figures.All(f => f.Pixels.All(p => p.Color == color)));
    }

    [Test]
    public void RemoveFigure_RemovesFigureFromCollection()
    {
        var color = 0xFF0000;
        var player = new Player(color);
        var figureToRemove = player.Figures.First();
            
        player.RemoveFigure(figureToRemove);
            
        Assert.AreEqual(2, player.Figures.Count);
        Assert.IsFalse(player.Figures.Contains(figureToRemove));
    }

    [Test]
    public void AddScore_IncreasesPlayerScore()
    {
        var color = 0xFF0000;
        var player = new Player(color);
        var initialScore = player.Score;
        var scoreToAdd = 100;
            
        player.AddScore(scoreToAdd);
            
        Assert.AreEqual(initialScore + scoreToAdd, player.Score);
    }
}
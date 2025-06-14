using BlockBlast_2._0.Models;

namespace BlockBlast_Test;

[TestFixture]
public class LeaderboardEntryTests
{
    [Test]
    public void LeaderboardEntry_Properties_AreSetCorrectly()
    {
        var date = DateTime.Now;
        
        var entry = new LeaderboardEntry
        {
            PlayerName = "TestPlayer",
            Score = 1000,
            Date = date,
            PlayerCount = 2,
            TimeLimit = 60
        };
        Assert.Multiple(() =>
        {
            Assert.That(entry.PlayerName, Is.EqualTo("TestPlayer"));
            Assert.That(entry.Score, Is.EqualTo(1000));
            Assert.That(entry.Date, Is.EqualTo(date));
            Assert.That(entry.PlayerCount, Is.EqualTo(2));
            Assert.That(entry.TimeLimit, Is.EqualTo(60));
        });
    }
}
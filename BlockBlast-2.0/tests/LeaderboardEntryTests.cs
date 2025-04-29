using BlockBlast_2._0.models;
using NUnit.Framework;

namespace BlockBlast_2._0.tests;

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
        
        Assert.AreEqual("TestPlayer", entry.PlayerName);
        Assert.AreEqual(1000, entry.Score);
        Assert.AreEqual(date, entry.Date);
        Assert.AreEqual(2, entry.PlayerCount);
        Assert.AreEqual(60, entry.TimeLimit);
    }
}
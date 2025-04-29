using System.Text.Json;
using BlockBlast_2._0.models;

namespace BlockBlast_2._0.controllers;

public class LeaderboardController
{
    private const string LeaderboardFile = "leaderboard.json";
    private List<LeaderboardEntry> _entries;

    public LeaderboardController()
    {
        _entries = LoadLeaderboard();
    }

    public void AddEntry(string playerName, int score, int playerCount, int timeLimit)
    {
        _entries.Add(new LeaderboardEntry
        {
            PlayerName = playerName,
            Score = score,
            Date = DateTime.Now,
            PlayerCount = playerCount,
            TimeLimit = timeLimit
        });
        
        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetLeaderboard(int playerCount, int timeLimit) =>
        _entries
            .Where(e => e.PlayerCount == playerCount && e.TimeLimit == timeLimit)
            .OrderByDescending(e => e.Score)
            .ThenBy(e => e.Date)
            .Take(10)
            .ToList();

    private List<LeaderboardEntry> LoadLeaderboard()
    {
        if (!File.Exists(LeaderboardFile))
            return [];
        try
        {
            var json = File.ReadAllText(LeaderboardFile);
            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private void SaveLeaderboard()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_entries, options);
        File.WriteAllText(LeaderboardFile, json);
    }
}
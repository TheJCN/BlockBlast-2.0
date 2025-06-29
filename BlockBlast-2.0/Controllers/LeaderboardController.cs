using System.Text.Json;
using BlockBlast_2._0.Models;

namespace BlockBlast_2._0.controllers;

public class LeaderboardController
{
    private const string LeaderboardFile = "leaderboard.json";
    private readonly List<LeaderboardEntry> _entries = LoadLeaderboard();
    private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };
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

    private static List<LeaderboardEntry> LoadLeaderboard()
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
        var json = JsonSerializer.Serialize(_entries, _serializerOptions);
        File.WriteAllText(LeaderboardFile, json);
    }
}
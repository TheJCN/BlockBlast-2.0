namespace BlockBlast_2._0.Models;

public class LeaderboardEntry
{
    public required string PlayerName { get; init; }
    public int Score { get; init; }
    public DateTime Date { get; init; }
    public int PlayerCount { get; init; }
    public int TimeLimit { get; init; }
}
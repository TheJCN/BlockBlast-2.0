using BlockBlast_2._0.controllers;
using BlockBlast_2._0.Models;

namespace BlockBlast_2._0.views;

public partial class LeaderboardForm : Form
{
    private readonly LeaderboardController _leaderboardController;

    public LeaderboardForm()
    {
        InitializeComponent();
        _leaderboardController = new LeaderboardController();
        InitializeLeaderboardTabs();
    }

    private void InitializeLeaderboardTabs()
    {
        var playerCounts = new[] { 1, 2, 4 };
        var timeLimits = new[] { 0, 5, 10, 15, 30 };

        foreach (var playerCount in playerCounts)
        {
            var playerTabPage = new TabPage
            {
                Text = GetPlayerTabText(playerCount),
                BackColor = Color.FromArgb(70, 70, 70),
                Padding = new Padding(10)
            };

            var timeTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Buttons,
                SizeMode = TabSizeMode.FillToRight
            };

            foreach (var timeLimit in timeLimits)
            {
                var timeTabPage = new TabPage
                {
                    Text = GetTimeLimitText(timeLimit),
                    BackColor = Color.FromArgb(70, 70, 70),
                    Padding = new Padding(10)
                };

                var leaderboard = _leaderboardController.GetLeaderboard(playerCount, timeLimit);
                var leaderboardPanel = CreateLeaderboardPanel(leaderboard);

                timeTabPage.Controls.Add(leaderboardPanel);
                timeTabControl.Controls.Add(timeTabPage);
            }

            playerTabPage.Controls.Add(timeTabControl);
            tabControlMain.Controls.Add(playerTabPage);
        }
    }

    private static string GetPlayerTabText(int playerCount)
    {
        return playerCount switch
        {
            1 => Resources.One_Player,
            2 => Resources.Two_Players,
            4 => Resources.Four_Players,
        };
    }

    private static string GetTimeLimitText(int timeLimit) => timeLimit == 0 ? Resources.No_Limit : string.Format(Resources.Limit, timeLimit);

    private static Panel CreateLeaderboardPanel(List<LeaderboardEntry> entries)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        var headerLabel = new Label
        {
            Text = Resources.Top_10_Results,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(headerLabel);

        if (entries.Count == 0)
        {
            var emptyLabel = new Label
            {
                Text = Resources.No_Entries,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.LightGray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(emptyLabel);
            return panel;
        }

        var tablePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            BackColor = Color.FromArgb(50, 50, 50),
            Padding = new Padding(5),
            Margin = new Padding(0, 0, 0, 10)
        };

        tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        AddHeaderCell(tablePanel, "#");
        AddHeaderCell(tablePanel, Resources.Clear_Player);
        AddHeaderCell(tablePanel, Resources.Clear_Score);
        AddHeaderCell(tablePanel, Resources.Clear_Date);

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            AddDataCell(tablePanel, (i + 1).ToString());
            AddDataCell(tablePanel, entry.PlayerName);
            AddDataCell(tablePanel, entry.Score.ToString());
            AddDataCell(tablePanel, entry.Date.ToString("dd.MM.yyyy"));
        }

        panel.Controls.Add(tablePanel);
        return panel;
    }

    private static void AddHeaderCell(TableLayoutPanel panel, string text)
    {
        var label = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Margin = new Padding(3)
        };
        panel.Controls.Add(label);
    }

    private static void AddDataCell(TableLayoutPanel panel, string text)
    {
        var label = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Margin = new Padding(3, 3, 3, 10)
        };
        panel.Controls.Add(label);
    }
}
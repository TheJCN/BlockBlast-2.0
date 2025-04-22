namespace BlockBlast_2._0;

public partial class MenuForm : Form
{
    public MenuForm()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        Text = "BlockBlast - Меню";
        ClientSize = new Size(600, 500);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(50, 50, 50);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var titleLabel = new Label
        {
            Text = "BlockBlast 2.0",
            Font = new Font("Segoe UI", 36, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 120
        };

        var singlePlayerBtn = CreateMenuButton("Одиночная игра", 0);
        var twoPlayersBtn = CreateMenuButton("Игра на двоих", 1);
        var fourPlayersBtn = CreateMenuButton("Игра вчетвером", 2);
        var leaderboardBtn = CreateMenuButton("Таблица лидеров", 3);

        var buttonsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1,
            Padding = new Padding(50, 20, 50, 50)
        };

        buttonsPanel.Controls.Add(singlePlayerBtn, 0, 0);
        buttonsPanel.Controls.Add(twoPlayersBtn, 0, 1);
        buttonsPanel.Controls.Add(fourPlayersBtn, 0, 2);
        buttonsPanel.Controls.Add(leaderboardBtn, 0, 3);

        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        buttonsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        Controls.Add(buttonsPanel);
        Controls.Add(titleLabel);
    }

    private Button CreateMenuButton(string text, int position)
    {
        var btn = new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = GetButtonColor(position),
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(10),
            Cursor = Cursors.Hand,
            Tag = position
        };

        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);

        btn.Click += MenuButton_Click;

        return btn;
    }

    private Color GetButtonColor(int position)
    {
        return position switch
        {
            0 => Color.FromArgb(0, 100, 0),    // Зеленый
            1 => Color.FromArgb(0, 0, 100),     // Синий
            2 => Color.FromArgb(100, 0, 100),    // Фиолетовый
            3 => Color.FromArgb(100, 100, 0),    // Золотой
            _ => Color.FromArgb(70, 70, 70)
        };
    }

    private void MenuButton_Click(object sender, EventArgs e)
    {
        if (sender is not Button button) return;

        switch ((int)button.Tag)
        {
            case 0: // Одиночная игра
                StartGame(1);
                break;
            case 1: // На двоих
                StartGame(2);
                break;
            case 2: // Вчетвером
                StartGame(4);
                break;
            case 3: // Таблица лидеров
                ShowLeaderboard();
                break;
        }
    }

    private void StartGame(int playerCount)
    {
        var gameForm = new GameForm(playerCount);
        gameForm.Show();
        Hide();
    }

    private void ShowLeaderboard()
    {
        MessageBox.Show("Таблица лидеров пока недоступна", "В разработке", 
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
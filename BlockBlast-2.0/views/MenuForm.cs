namespace BlockBlast_2._0.views
{
    public partial class MenuForm : Form
    {
        private ComboBox? timeComboBox;
        private int selectedTimeLimit;

        public MenuForm()
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            Text = "BlockBlast - Меню";
            ClientSize = new Size(800, 600);
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

            var settingsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(20)
            };

            var timeLabel = new Label
            {
                Text = "Лимит времени на ход:",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            timeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 12),
                Width = 150,
                Location = new Point(200, 17)
            };
            timeComboBox.Items.AddRange(new object[] { 
                "Без ограничения", 
                "5 секунд", 
                "10 секунд", 
                "15 секунд", 
                "30 секунд" 
            });
            timeComboBox.SelectedIndex = 0;
            timeComboBox.SelectedIndexChanged += (_, _) => 
            {
                selectedTimeLimit = timeComboBox.SelectedIndex switch
                {
                    1 => 5,
                    2 => 10,
                    3 => 15,
                    4 => 30,
                    _ => 0
                };
            };

            settingsPanel.Controls.Add(timeLabel);
            settingsPanel.Controls.Add(timeComboBox);

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
            Controls.Add(settingsPanel);
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

            btn.Click += MenuButton_Click!;

            return btn;
        }

        private Color GetButtonColor(int position)
        {
            return position switch
            {
                0 => Color.FromArgb(0, 100, 0),
                1 => Color.FromArgb(0, 0, 100),
                2 => Color.FromArgb(100, 0, 100),
                3 => Color.FromArgb(100, 100, 0),
                _ => Color.FromArgb(70, 70, 70)
            };
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button button) return;

            switch ((int)button.Tag!)
            {
                case 0:
                    StartGame(1, selectedTimeLimit);
                    break;
                case 1:
                    StartGame(2, selectedTimeLimit);
                    break;
                case 2:
                    StartGame(4, selectedTimeLimit);
                    break;
                case 3:
                    ShowLeaderboard();
                    break;
            }
        }

        private void StartGame(int playerCount, int timeLimit)
        {
            var gameForm = new GameForm(playerCount, timeLimit);
            gameForm.Show();
            Hide();
        }
        
        private void ShowLeaderboard()
        {
            var leaderboardForm = new LeaderboardForm();
            leaderboardForm.ShowDialog();
        }
    }
}
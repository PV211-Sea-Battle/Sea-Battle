using Models;

namespace Sea_Battle
{
    public partial class MainPage : Form
    {
        public List<Game> games;
        private int? previousSelectedIndex;
        public MainPage()
        {
            InitializeComponent();

            joinGameButton.EnabledChanged += CurrentUser.ButtonEnabledChanged;
            joinGameButton.Enabled = false;

            welcomeLabel.Text = "Welcome, " + CurrentUser.user.Login;

            ServerListLoad();
        }

        public async void ServerListLoad()
        {
            try
            {
                games = [];
                string header = "GAME LIST";

                var request = new Request()
                {
                    Header = header
                };

                Response response = await CurrentUser.SendMessageAsync(request);

                games = response.Games;

                if (serverList.SelectedIndices.Count == 1)
                {
                    previousSelectedIndex = serverList.SelectedIndices[0];
                }
                else
                {
                    previousSelectedIndex = null;
                }

                foreach (ListViewItem item in serverList.Items)
                {
                    serverList.Items.Remove(item);
                }

                foreach (Game game in games)
                {
                    string[] row =
                    [
                        game.Name,
                    game.IsPrivate ? "+" : string.Empty,
                    game.User.Login
                    ];

                    serverList.Items.Add(new ListViewItem(row));
                }

                if (previousSelectedIndex is not null && previousSelectedIndex < serverList.Items.Count)
                {
                    serverList.Items[previousSelectedIndex.Value].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void createGameButton_Click(object sender, EventArgs e)
        {
            //окно создания игры
        }

        private void joinGameButton_Click(object sender, EventArgs e)
        {
            Game selectedGame = games[serverList.SelectedIndices[0]];

            if (selectedGame.IsPrivate)
            {
                //окно подключения к игре с паролем
            }
            else
            {
                //окно подготовки к игре
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ServerListLoad();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer timer = new();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverList.SelectedIndices.Count == 1)
            {
                joinGameButton.Enabled = true;
            }

            else
            {
                joinGameButton.Enabled = false;
            }
        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
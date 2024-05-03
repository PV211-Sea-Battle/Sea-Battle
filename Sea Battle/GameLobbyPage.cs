using Models;

namespace Sea_Battle
{
    public partial class GameLobbyPage : Form
    {
        public GameLobbyPage()
        {
            InitializeComponent();

            connectButton.EnabledChanged += CurrentUser.ButtonEnabledChanged;

            gameNameField.Text = CurrentUser.game.Name;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passwordField.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(passwordField.Text))
                {
                    MessageBox.Show("Enter the password", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Request request = new()
                {
                    Header = "JOIN",
                    User = CurrentUser.user,
                    Game = CurrentUser.game,
                    EnteredGamePassword = passwordField.Text
                };

                await CurrentUser.SendMessageAsync(request);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CurrentUser.form = new MainPage();
            Close();
        }
    }
}

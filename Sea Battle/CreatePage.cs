using Models;

namespace Sea_Battle
{
    public partial class CreatePage : Form
    {
        public CreatePage()
        {
            InitializeComponent();

            createButton.EnabledChanged += CurrentUser.ButtonEnabledChanged;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CurrentUser.form = new MainPage();
            Close();
        }

        private async void createButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nameField.Text))
                {
                    MessageBox.Show("Enter the name", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if(string.IsNullOrWhiteSpace(nameField.Text) && checkBox1.Checked && string.IsNullOrWhiteSpace(passwordField.Text))
                {
                    MessageBox.Show("Enter the name and password", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (checkBox1.Checked && string.IsNullOrWhiteSpace(passwordField.Text))
                {
                    MessageBox.Show("Enter the password", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Request request = new()
                {
                    Header = "CREATE",
                    User = CurrentUser.user,
                    Game =  new Game
                    {
                        Name = nameField.Text,
                        IsPrivate = checkBox1.Checked,
                        Password = passwordField.Text,
                        HostUserId = CurrentUser.user.Id //в этом не уверен
                    },
                };

                await CurrentUser.SendMessageAsync(request);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            passwordField.Clear();
            passwordField.Enabled = checkBox1.Checked;
        }
    }
}

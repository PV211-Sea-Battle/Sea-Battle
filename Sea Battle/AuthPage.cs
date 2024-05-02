using Models;

namespace Sea_Battle
{
    public partial class AuthPage : Form
    {
        private int _port;
        private string _addr;

        public AuthPage()
        {
            InitializeComponent();

            _port = int.Parse(portField.Text);
            _addr = ipField.Text;

            connectButton.Enabled = false;
        }

        private void ClearBoxes()
        {
            loginField.Clear();
            passwordField.Clear();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(_addr != null && _port != 0)
                {
                    string header = "SIGN IN";
                    string login = loginField.Text;
                    string password = passwordField.Text;

                    if (string.IsNullOrWhiteSpace(login))
                    {
                        MessageBox.Show("Enter the login", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    else if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Enter the password", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    else
                    {
                        var request = new Request()
                        {
                            Header = header,
                            User = new User()
                            {
                                Login = login,
                                Password = password
                            }
                        };

                        CurrentUser.address = _addr;
                        CurrentUser.port = _port;
                        Response response = await CurrentUser.SendMessageAsync(request);

                        //проверку нужно поменять, я не особо знаю как
                        if (response.ErrorMessage == null)
                        {
                            CurrentUser.User = response.User;
                            MainPage mainPage = new MainPage();
                            mainPage.Show();
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect login or password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearBoxes();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You aren't connected to the server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearBoxes();
            }
        }

        private async void reginButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_addr != null && _port != 0)
                {
                    string header = "REGISTER";
                    string login = loginField.Text;
                    string password = passwordField.Text;

                    if (string.IsNullOrWhiteSpace(login))
                    {
                        MessageBox.Show("Entry the login", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    else if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Entry the password", "Caption!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    else
                    {
                        var request = new Request()
                        {
                            Header = header,
                            User = new User()
                            {
                                Login = login,
                                Password = password
                            }
                        };

                        CurrentUser.address = _addr;
                        CurrentUser.port = _port;
                        Response response = await CurrentUser.SendMessageAsync(request);

                        //проверку нужно поменять, я не особо знаю как
                        if (response.ErrorMessage == null)
                        {
                            MessageBox.Show("You created new account!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearBoxes();
                        }
                        else
                        {
                            MessageBox.Show("Account with this login already exists!", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearBoxes();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You aren't connected to the server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearBoxes();
            }
        }

        private void AuthPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _port = int.Parse(portField.Text);
                _addr = ipField.Text;

                connectButton.Enabled = false;
                disconnectButton.Enabled = true;
            }
            catch
            {
                MessageBox.Show("IP Address Field or Port Field are empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            _port = 0;
            _addr = string.Empty;

            ipField.Clear();
            portField.Clear();

            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
        }

        private void Button_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.ForeColor = button.Enabled ? Color.White : Color.Gray;
                button.BackColor = button.Enabled ? Color.Black : Color.DimGray;
            }
        }
    }
}
﻿using Models;

namespace Sea_Battle
{
    public partial class AuthPage : Form
    {
        public AuthPage()
        {
            InitializeComponent();

            connectButton.EnabledChanged += CurrentUser.ButtonEnabledChanged;
            disconnectButton.EnabledChanged += CurrentUser.ButtonEnabledChanged;

            CurrentUser.port = int.Parse(portField.Text);
            CurrentUser.address = ipField.Text;

            ipField.ReadOnly = true;
            portField.ReadOnly = true;

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

                    Response response = await CurrentUser.SendMessageAsync(request);

                    CurrentUser.user = response.User;
                    CurrentUser.form = new MainPage();
                    Close();
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

                    Response response = await CurrentUser.SendMessageAsync(request);

                    CurrentUser.user = response.User;
                    CurrentUser.form = new MainPage();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearBoxes();
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentUser.port = int.Parse(portField.Text);
                CurrentUser.address = ipField.Text;

                ipField.ReadOnly = true;
                portField.ReadOnly = true;

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
            CurrentUser.port = null;
            CurrentUser.address = null;

            ipField.Clear();
            portField.Clear();

            ipField.ReadOnly = false;
            portField.ReadOnly = false;

            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
        }
    }
}
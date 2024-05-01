#pragma warning disable SYSLIB0011
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace Sea_Battle
{
    public partial class AuthPage : Form
    {
        private int _port;
        private IPAddress _addr;
        private IPEndPoint _ep;

        private TcpClient _client;
        private BinaryFormatter _bf;

        public AuthPage()
        {
            InitializeComponent();

            _port = 9001;
            _addr = IPAddress.Parse("127.0.0.1");
            _ep = new IPEndPoint(_addr, _port);
            _bf = new BinaryFormatter();
        }

        private void ClearBoxes()
        {
            loginField.Clear();
            passwordField.Clear();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string actionKey = "LOGIN";
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
                    ActionKey = actionKey,
                    User = new User()
                    {
                        Login = login,
                        Password = password
                    }
                };

                _client = new TcpClient();
                _client.Connect(_ep);
                NetworkStream ns = _client.GetStream();
                _bf.Serialize(ns, request);

                Response response = (Response)_bf.Deserialize(ns);
                string? message = response.Message;

                if (message == "OK")
                {
                    //переход на главную страницу
                    Hide();
                }

                else
                {
                    MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearBoxes();
                }

                ns.Close();
                _client.Close();
            }
        }

        private void reginButton_Click(object sender, EventArgs e)
        {
            string actionKey = "REGISTER";
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
                    ActionKey = actionKey,
                    User = new User()
                    {
                        Login = login,
                        Password = password
                    }
                };

                _client = new TcpClient();
                _client.Connect(_ep);
                NetworkStream ns = _client.GetStream();
                _bf.Serialize(ns, request);

                Response response = (Response)_bf.Deserialize(ns);
                string? message = response.Message;
                if (message == "OK")
                {
                    MessageBox.Show("You created new account", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearBoxes();
                }
                else
                {
                    MessageBox.Show("Account with this login already exists", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearBoxes();
                }

                ns.Close();
                _client.Close();
            }
        }

        private void AuthPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
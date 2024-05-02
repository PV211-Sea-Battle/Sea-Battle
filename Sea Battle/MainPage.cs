using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using Models;

namespace Sea_Battle
{
    public partial class MainPage : Form
    {
        public List<Game> games;
        private int previousSelectedIndex;
        public MainPage()
        {
            InitializeComponent();
            welcomeLabel.Text = "Welcome, " + CurrentUser.User.Login;
        }

        public async void ServerListLoad()
        {
            previousSelectedIndex = serverList.SelectedIndex;

            games = new List<Game>();
            string header = "GAME LIST";

            var request = new Request()
            {
                Header = header
            };

            Response response = await CurrentUser.SendMessageAsync(request);

            games = response.Games;
            serverList.DataSource = games;
            //в классе game нужно добавить еще поля по типу, сколько человек приконектилось
            //lisеbox можно при желании заменить на listview с колонками и прочим, если надо
            serverList.DisplayMember = "Name";


            if (previousSelectedIndex >= 0 && previousSelectedIndex < serverList.Items.Count)
            {
                serverList.SelectedIndex = previousSelectedIndex;
            }
        }

        private void createGameButton_Click(object sender, EventArgs e)
        {
            //окно создания игры
        }

        private void joinGameButton_Click(object sender, EventArgs e)
        {
            string gameText = serverList.GetItemText(serverList.SelectedItem);
            Game selectedGame = games.FirstOrDefault(game => game.Name == gameText);
            string gameName = selectedGame.Name;

            if(selectedGame.IsPrivate)
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
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverList.SelectedIndex != -1)
            {
                joinGameButton.Enabled = true;
            }

            else
            {
                joinGameButton.Enabled = false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sea_Battle
{
    public partial class GameLobbyPage : Form
    {
        bool field = false;
        public GameLobbyPage()
        {
            InitializeComponent();
            passwordField.UseSystemPasswordChar = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void passwordField_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                passwordField.UseSystemPasswordChar = false;
            }
            else
                passwordField.UseSystemPasswordChar = true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void passwordField_MouseDown(object sender, MouseEventArgs e)
        {
            if (!field)
            {
                passwordField.Clear();
                field = true;

            }
            if (checkBox1.Checked)
            {
                passwordField.UseSystemPasswordChar = false;
            }
            else
                passwordField.UseSystemPasswordChar = true;

        }
    }
}

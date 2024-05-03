namespace Sea_Battle
{
    partial class AuthPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthPage));
            groupBox1 = new GroupBox();
            disconnectButton = new Button();
            connectButton = new Button();
            portField = new TextBox();
            ipField = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            passwordField = new TextBox();
            loginField = new TextBox();
            label1 = new Label();
            reginButton = new Button();
            loginButton = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Desktop;
            groupBox1.Controls.Add(disconnectButton);
            groupBox1.Controls.Add(connectButton);
            groupBox1.Controls.Add(portField);
            groupBox1.Controls.Add(ipField);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(passwordField);
            groupBox1.Controls.Add(loginField);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(reginButton);
            groupBox1.Controls.Add(loginButton);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(392, 689);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Authorization";
            // 
            // disconnectButton
            // 
            disconnectButton.BackColor = SystemColors.Desktop;
            disconnectButton.Location = new Point(202, 651);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(184, 32);
            disconnectButton.TabIndex = 13;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = false;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // connectButton
            // 
            connectButton.BackColor = SystemColors.Desktop;
            connectButton.ForeColor = Color.White;
            connectButton.Location = new Point(6, 651);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(190, 32);
            connectButton.TabIndex = 12;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // portField
            // 
            portField.BackColor = SystemColors.Desktop;
            portField.ForeColor = Color.Lime;
            portField.Location = new Point(49, 620);
            portField.Name = "portField";
            portField.Size = new Size(337, 25);
            portField.TabIndex = 11;
            portField.Text = "9001";
            portField.TextAlign = HorizontalAlignment.Center;
            // 
            // ipField
            // 
            ipField.BackColor = SystemColors.Desktop;
            ipField.ForeColor = Color.Lime;
            ipField.Location = new Point(89, 587);
            ipField.Name = "ipField";
            ipField.Size = new Size(297, 25);
            ipField.TabIndex = 10;
            ipField.Text = "127.0.0.1";
            ipField.TextAlign = HorizontalAlignment.Center;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 620);
            label5.Name = "label5";
            label5.Size = new Size(37, 19);
            label5.TabIndex = 9;
            label5.Text = "Port:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 593);
            label4.Name = "label4";
            label4.Size = new Size(77, 19);
            label4.TabIndex = 8;
            label4.Text = "IP Address:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Black", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label3.ForeColor = Color.FromArgb(255, 128, 0);
            label3.Location = new Point(70, 210);
            label3.Name = "label3";
            label3.Size = new Size(243, 50);
            label3.TabIndex = 7;
            label3.Text = "SEA BATTLE";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(63, 46);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(250, 161);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(63, 381);
            label2.Name = "label2";
            label2.Size = new Size(67, 19);
            label2.TabIndex = 5;
            label2.Text = "Password";
            // 
            // passwordField
            // 
            passwordField.BackColor = SystemColors.Desktop;
            passwordField.ForeColor = Color.White;
            passwordField.Location = new Point(63, 403);
            passwordField.Name = "passwordField";
            passwordField.Size = new Size(250, 25);
            passwordField.TabIndex = 4;
            passwordField.TextAlign = HorizontalAlignment.Center;
            passwordField.UseSystemPasswordChar = true;
            // 
            // loginField
            // 
            loginField.BackColor = SystemColors.Desktop;
            loginField.ForeColor = Color.White;
            loginField.Location = new Point(63, 340);
            loginField.Name = "loginField";
            loginField.Size = new Size(250, 25);
            loginField.TabIndex = 3;
            loginField.TextAlign = HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 318);
            label1.Name = "label1";
            label1.Size = new Size(43, 19);
            label1.TabIndex = 2;
            label1.Text = "Login";
            // 
            // reginButton
            // 
            reginButton.BackColor = Color.Black;
            reginButton.ForeColor = Color.White;
            reginButton.Location = new Point(191, 453);
            reginButton.Name = "reginButton";
            reginButton.Size = new Size(122, 52);
            reginButton.TabIndex = 1;
            reginButton.Text = "Register";
            reginButton.UseVisualStyleBackColor = false;
            reginButton.Click += reginButton_Click;
            // 
            // loginButton
            // 
            loginButton.BackColor = Color.Black;
            loginButton.ForeColor = Color.White;
            loginButton.Location = new Point(63, 453);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(122, 52);
            loginButton.TabIndex = 0;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = false;
            loginButton.Click += loginButton_Click;
            // 
            // AuthPage
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(416, 713);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AuthPage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Authorization";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Button reginButton;
        private Button loginButton;
        private Label label2;
        private TextBox passwordField;
        private TextBox loginField;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label3;
        private TextBox portField;
        private TextBox ipField;
        private Label label5;
        private Label label4;
        private Button disconnectButton;
        private Button connectButton;
    }
}
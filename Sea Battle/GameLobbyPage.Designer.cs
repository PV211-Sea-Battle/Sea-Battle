namespace Sea_Battle
{
    partial class GameLobbyPage
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
            groupBox1 = new GroupBox();
            label4 = new Label();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            passwordField = new TextBox();
            cancelButton = new Button();
            connectButton = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Desktop;
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(passwordField);
            groupBox1.Controls.Add(cancelButton);
            groupBox1.Controls.Add(connectButton);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(392, 695);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Connect";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // label4
            // 
            label4.BorderStyle = BorderStyle.Fixed3D;
            label4.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label4.Location = new Point(63, 292);
            label4.Name = "label4";
            label4.Size = new Size(259, 44);
            label4.TabIndex = 10;
            label4.Text = "тутБуде Логін";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 272);
            label1.Name = "label1";
            label1.Size = new Size(98, 20);
            label1.TabIndex = 9;
            label1.Text = "Game/Lobby:";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(63, 403);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(132, 24);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "show password";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Black", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label3.ForeColor = Color.FromArgb(0, 192, 0);
            label3.Location = new Point(33, 210);
            label3.Name = "label3";
            label3.Size = new Size(306, 62);
            label3.TabIndex = 7;
            label3.Text = "SEA BATTLE";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.AuthPic;
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
            label2.Location = new Point(63, 347);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 5;
            label2.Text = "Password";
            // 
            // passwordField
            // 
            passwordField.BackColor = SystemColors.Desktop;
            passwordField.ForeColor = Color.White;
            passwordField.Location = new Point(63, 370);
            passwordField.Name = "passwordField";
            passwordField.Size = new Size(250, 27);
            passwordField.TabIndex = 4;
            passwordField.Text = "Enter password of game";
            passwordField.UseSystemPasswordChar = true;
            passwordField.TextChanged += passwordField_TextChanged;
            passwordField.MouseDown += passwordField_MouseDown;
            // 
            // cancelButton
            // 
            cancelButton.BackColor = Color.Black;
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(191, 453);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(122, 52);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // connectButton
            // 
            connectButton.BackColor = Color.Black;
            connectButton.ForeColor = Color.White;
            connectButton.Location = new Point(63, 453);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(122, 52);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // GameLobbyPage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 713);
            Controls.Add(groupBox1);
            Name = "GameLobbyPage";
            Text = "GameLobbyPage";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label4;
        private Label label1;
        private CheckBox checkBox1;
        private Label label3;
        private PictureBox pictureBox1;
        private Label label2;
        private TextBox passwordField;
        private Button cancelButton;
        private Button connectButton;
    }
}
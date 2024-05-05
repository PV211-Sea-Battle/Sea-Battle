namespace Sea_Battle
{
    partial class ConnectPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectPage));
            groupBox1 = new GroupBox();
            gameNameField = new TextBox();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label2 = new Label();
            passwordField = new TextBox();
            cancelButton = new Button();
            connectButton = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Desktop;
            groupBox1.Controls.Add(gameNameField);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(passwordField);
            groupBox1.Controls.Add(cancelButton);
            groupBox1.Controls.Add(connectButton);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 11);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(340, 336);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Connect";
            // 
            // gameNameField
            // 
            gameNameField.BackColor = SystemColors.Desktop;
            gameNameField.ForeColor = Color.White;
            gameNameField.Location = new Point(46, 85);
            gameNameField.Margin = new Padding(3, 2, 3, 2);
            gameNameField.Name = "gameNameField";
            gameNameField.ReadOnly = true;
            gameNameField.Size = new Size(250, 25);
            gameNameField.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(46, 64);
            label1.Name = "label1";
            label1.Size = new Size(129, 19);
            label1.TabIndex = 9;
            label1.Text = "Game/Lobby name:";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(46, 177);
            checkBox1.Margin = new Padding(3, 2, 3, 2);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(122, 23);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "show password";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 127);
            label2.Name = "label2";
            label2.Size = new Size(67, 19);
            label2.TabIndex = 5;
            label2.Text = "Password";
            // 
            // passwordField
            // 
            passwordField.BackColor = SystemColors.Desktop;
            passwordField.ForeColor = Color.White;
            passwordField.Location = new Point(46, 148);
            passwordField.Margin = new Padding(3, 2, 3, 2);
            passwordField.Name = "passwordField";
            passwordField.Size = new Size(250, 25);
            passwordField.TabIndex = 4;
            passwordField.UseSystemPasswordChar = true;
            // 
            // cancelButton
            // 
            cancelButton.BackColor = Color.Black;
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(174, 218);
            cancelButton.Margin = new Padding(3, 2, 3, 2);
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
            connectButton.Location = new Point(46, 218);
            connectButton.Margin = new Padding(3, 2, 3, 2);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(122, 52);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = false;
            connectButton.Click += connectButton_Click;
            // 
            // ConnectPage
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(364, 358);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "ConnectPage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Connect to game";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private CheckBox checkBox1;
        private Label label2;
        private TextBox passwordField;
        private Button cancelButton;
        private Button connectButton;
        private TextBox gameNameField;
    }
}
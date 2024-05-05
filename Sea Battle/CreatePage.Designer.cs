namespace Sea_Battle
{
    partial class CreatePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatePage));
            nameField = new TextBox();
            passwordField = new TextBox();
            checkBox1 = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            cancelButton = new Button();
            createButton = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // nameField
            // 
            nameField.BackColor = SystemColors.Desktop;
            nameField.ForeColor = Color.White;
            nameField.Location = new Point(46, 85);
            nameField.Name = "nameField";
            nameField.Size = new Size(250, 25);
            nameField.TabIndex = 0;
            // 
            // passwordField
            // 
            passwordField.BackColor = SystemColors.Desktop;
            passwordField.ForeColor = Color.White;
            passwordField.Location = new Point(46, 164);
            passwordField.Name = "passwordField";
            passwordField.Size = new Size(250, 25);
            passwordField.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(46, 116);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(163, 23);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Create a private game";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(46, 64);
            label1.Name = "label1";
            label1.Size = new Size(129, 19);
            label1.TabIndex = 3;
            label1.Text = "Game/Lobby name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 142);
            label2.Name = "label2";
            label2.Size = new Size(70, 19);
            label2.TabIndex = 4;
            label2.Text = "Password:";
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Desktop;
            groupBox1.Controls.Add(cancelButton);
            groupBox1.Controls.Add(createButton);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(nameField);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(passwordField);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(340, 336);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Create";
            // 
            // cancelButton
            // 
            cancelButton.BackColor = Color.Black;
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(174, 216);
            cancelButton.Margin = new Padding(3, 2, 3, 2);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(122, 52);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // createButton
            // 
            createButton.BackColor = Color.Black;
            createButton.ForeColor = Color.White;
            createButton.Location = new Point(46, 216);
            createButton.Margin = new Padding(3, 2, 3, 2);
            createButton.Name = "createButton";
            createButton.Size = new Size(122, 52);
            createButton.TabIndex = 5;
            createButton.Text = "Create";
            createButton.UseVisualStyleBackColor = false;
            createButton.Click += createButton_Click;
            // 
            // CreatePage
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
            Name = "CreatePage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create a game";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox nameField;
        private TextBox passwordField;
        private CheckBox checkBox1;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
        private Button cancelButton;
        private Button createButton;
    }
}
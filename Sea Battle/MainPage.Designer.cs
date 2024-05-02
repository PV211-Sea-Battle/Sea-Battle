namespace Sea_Battle
{
    partial class MainPage
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            groupBox1 = new GroupBox();
            serverList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            groupBox2 = new GroupBox();
            joinGameButton = new Button();
            createGameButton = new Button();
            welcomeLabel = new Label();
            timer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(serverList);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1148, 553);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server list";
            // 
            // serverList
            // 
            serverList.BackColor = SystemColors.Desktop;
            serverList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            serverList.Dock = DockStyle.Fill;
            serverList.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            serverList.ForeColor = Color.White;
            serverList.Location = new Point(3, 21);
            serverList.MultiSelect = false;
            serverList.Name = "serverList";
            serverList.Size = new Size(1142, 529);
            serverList.TabIndex = 1;
            serverList.UseCompatibleStateImageBehavior = false;
            serverList.View = View.Details;
            serverList.SelectedIndexChanged += serverList_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.TextAlign = HorizontalAlignment.Center;
            columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Password";
            columnHeader2.TextAlign = HorizontalAlignment.Center;
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Host";
            columnHeader3.TextAlign = HorizontalAlignment.Center;
            columnHeader3.Width = 250;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(joinGameButton);
            groupBox2.Controls.Add(createGameButton);
            groupBox2.Controls.Add(welcomeLabel);
            groupBox2.ForeColor = Color.White;
            groupBox2.Location = new Point(12, 571);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1148, 80);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Options";
            // 
            // joinGameButton
            // 
            joinGameButton.BackColor = SystemColors.Desktop;
            joinGameButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            joinGameButton.ForeColor = Color.White;
            joinGameButton.Location = new Point(790, 14);
            joinGameButton.Name = "joinGameButton";
            joinGameButton.Size = new Size(173, 60);
            joinGameButton.TabIndex = 2;
            joinGameButton.Text = "Join Game";
            joinGameButton.UseVisualStyleBackColor = false;
            joinGameButton.Click += joinGameButton_Click;
            // 
            // createGameButton
            // 
            createGameButton.BackColor = SystemColors.Desktop;
            createGameButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            createGameButton.ForeColor = Color.White;
            createGameButton.Location = new Point(969, 14);
            createGameButton.Name = "createGameButton";
            createGameButton.Size = new Size(173, 60);
            createGameButton.TabIndex = 1;
            createGameButton.Text = "Create Game";
            createGameButton.UseVisualStyleBackColor = false;
            createGameButton.Click += createGameButton_Click;
            // 
            // welcomeLabel
            // 
            welcomeLabel.AutoSize = true;
            welcomeLabel.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            welcomeLabel.Location = new Point(6, 21);
            welcomeLabel.Name = "welcomeLabel";
            welcomeLabel.Size = new Size(0, 50);
            welcomeLabel.TabIndex = 0;
            // 
            // timer
            // 
            timer.Tick += timer_Tick;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1172, 663);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainPage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main page";
            FormClosing += MainPage_FormClosing;
            Load += MainPage_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label welcomeLabel;
        private Button createGameButton;
        private Button joinGameButton;
        private System.Windows.Forms.Timer timer;
        private ListView serverList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
    }
}
namespace DynamicWallpaperWin
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			pictureBox1 = new PictureBox();
			buttonOK = new Button();
			labelAppName = new Label();
			labelAppVersion = new Label();
			labelCopyright = new Label();
			linkLabelGH = new LinkLabel();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.Image = Properties.Resources.dynwallpaper_logo;
			pictureBox1.Location = new Point(13, 13);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(64, 64);
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// buttonOK
			// 
			buttonOK.Location = new Point(206, 98);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(75, 23);
			buttonOK.TabIndex = 1;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += buttonOK_Click;
			// 
			// labelAppName
			// 
			labelAppName.AutoSize = true;
			labelAppName.Location = new Point(83, 13);
			labelAppName.Name = "labelAppName";
			labelAppName.Size = new Size(106, 15);
			labelAppName.TabIndex = 2;
			labelAppName.Text = "{applicationName}";
			labelAppName.Click += labelAppName_Click;
			// 
			// labelAppVersion
			// 
			labelAppVersion.AutoSize = true;
			labelAppVersion.Location = new Point(83, 37);
			labelAppVersion.Name = "labelAppVersion";
			labelAppVersion.Size = new Size(112, 15);
			labelAppVersion.TabIndex = 3;
			labelAppVersion.Text = "{applicationVersion}";
			// 
			// labelCopyright
			// 
			labelCopyright.AutoSize = true;
			labelCopyright.Location = new Point(13, 80);
			labelCopyright.Name = "labelCopyright";
			labelCopyright.Size = new Size(127, 15);
			labelCopyright.TabIndex = 4;
			labelCopyright.Text = "{applicationCopyright}";
			// 
			// linkLabelGH
			// 
			linkLabelGH.AutoSize = true;
			linkLabelGH.Location = new Point(13, 109);
			linkLabelGH.Name = "linkLabelGH";
			linkLabelGH.Size = new Size(113, 15);
			linkLabelGH.TabIndex = 5;
			linkLabelGH.TabStop = true;
			linkLabelGH.Text = "{applicationGHLink}";
			// 
			// AboutForm
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(294, 134);
			Controls.Add(linkLabelGH);
			Controls.Add(labelCopyright);
			Controls.Add(labelAppVersion);
			Controls.Add(labelAppName);
			Controls.Add(buttonOK);
			Controls.Add(pictureBox1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutForm";
			Padding = new Padding(10);
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Dynamic Wallpaper for Windows";
			Load += AboutForm_Load;
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private PictureBox pictureBox1;
		private Button buttonOK;
		private Label labelAppName;
		private Label labelAppVersion;
		private Label labelCopyright;
		private LinkLabel linkLabelGH;
	}
}

namespace SEConsoleLab
{
	partial class Form_LCDDisplay
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
			this.components = new System.ComponentModel.Container();
			this.LCD0 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			this.textBox1 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.LCD0)).BeginInit();
			this.SuspendLayout();
			// 
			// LCD0
			// 
			this.LCD0.Location = new System.Drawing.Point(12, 12);
			this.LCD0.Name = "LCD0";
			this.LCD0.Size = new System.Drawing.Size(800, 800);
			this.LCD0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.LCD0.TabIndex = 0;
			this.LCD0.TabStop = false;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(873, 79);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(186, 76);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// timer2
			// 
			this.timer2.Enabled = true;
			this.timer2.Interval = 60;
			this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(873, 192);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(400, 400);
			this.textBox1.TabIndex = 2;
			// 
			// Form_LCDDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1486, 859);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.LCD0);
			this.DoubleBuffered = true;
			this.Name = "Form_LCDDisplay";
			this.Text = "Form_LCDDisplay";
			this.Load += new System.EventHandler(this.Form_LCDDisplay_Load);
			((System.ComponentModel.ISupportInitialize)(this.LCD0)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox LCD0;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Timer timer2;
		private System.Windows.Forms.TextBox textBox1;
	}
}
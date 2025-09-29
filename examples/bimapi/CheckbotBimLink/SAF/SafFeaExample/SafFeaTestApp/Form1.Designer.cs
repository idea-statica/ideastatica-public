namespace SafFeaTestApp
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button2 = new Button();
			button3 = new Button();
			SuspendLayout();
			// 
			// button2
			// 
			button2.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
			button2.Location = new Point(195, 166);
			button2.Name = "button2";
			button2.Size = new Size(421, 97);
			button2.TabIndex = 1;
			button2.Text = "Run Checkbot";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// button3
			// 
			button3.Location = new Point(327, 280);
			button3.Name = "button3";
			button3.Size = new Size(146, 23);
			button3.TabIndex = 2;
			button3.Text = "Clear Existing Project";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(button3);
			Controls.Add(button2);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
		}

		#endregion
		private Button button2;
		private Button button3;
	}
}

namespace AudioLib
{
	public partial class RealtimeHostConfigEditor
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
			this.comboBoxHost = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxInput = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxSamplerate = new System.Windows.Forms.TextBox();
			this.textBoxInputChannels = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxOutputChannels = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxLatency = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.comboBoxOutput = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelASIO = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBoxHost
			// 
			this.comboBoxHost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxHost.FormattingEnabled = true;
			this.comboBoxHost.Location = new System.Drawing.Point(154, 33);
			this.comboBoxHost.Name = "comboBoxHost";
			this.comboBoxHost.Size = new System.Drawing.Size(270, 21);
			this.comboBoxHost.TabIndex = 0;
			this.comboBoxHost.SelectedIndexChanged += new System.EventHandler(this.comboBoxHost_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(56, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Device Type";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(56, 155);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Input Device";
			// 
			// comboBoxInput
			// 
			this.comboBoxInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxInput.FormattingEnabled = true;
			this.comboBoxInput.Location = new System.Drawing.Point(154, 152);
			this.comboBoxInput.Name = "comboBoxInput";
			this.comboBoxInput.Size = new System.Drawing.Size(270, 21);
			this.comboBoxInput.TabIndex = 2;
			this.comboBoxInput.SelectedIndexChanged += new System.EventHandler(this.comboBoxInput_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(64, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Samplerate";
			// 
			// textBoxSamplerate
			// 
			this.textBoxSamplerate.Location = new System.Drawing.Point(154, 60);
			this.textBoxSamplerate.Name = "textBoxSamplerate";
			this.textBoxSamplerate.Size = new System.Drawing.Size(79, 20);
			this.textBoxSamplerate.TabIndex = 6;
			// 
			// textBoxInputChannels
			// 
			this.textBoxInputChannels.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.textBoxInputChannels.Enabled = false;
			this.textBoxInputChannels.Location = new System.Drawing.Point(154, 179);
			this.textBoxInputChannels.Name = "textBoxInputChannels";
			this.textBoxInputChannels.Size = new System.Drawing.Size(46, 20);
			this.textBoxInputChannels.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(73, 182);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Channels";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(73, 265);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(51, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "Channels";
			// 
			// textBoxOutputChannels
			// 
			this.textBoxOutputChannels.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.textBoxOutputChannels.Enabled = false;
			this.textBoxOutputChannels.Location = new System.Drawing.Point(154, 262);
			this.textBoxOutputChannels.Name = "textBoxOutputChannels";
			this.textBoxOutputChannels.Size = new System.Drawing.Size(46, 20);
			this.textBoxOutputChannels.TabIndex = 16;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(30, 89);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(94, 13);
			this.label8.TabIndex = 13;
			this.label8.Text = "Latency (Samples)";
			// 
			// textBoxLatency
			// 
			this.textBoxLatency.Location = new System.Drawing.Point(154, 86);
			this.textBoxLatency.Name = "textBoxLatency";
			this.textBoxLatency.Size = new System.Drawing.Size(79, 20);
			this.textBoxLatency.TabIndex = 12;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(48, 238);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(76, 13);
			this.label9.TabIndex = 11;
			this.label9.Text = "Output Device";
			// 
			// comboBoxOutput
			// 
			this.comboBoxOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutput.FormattingEnabled = true;
			this.comboBoxOutput.Location = new System.Drawing.Point(154, 235);
			this.comboBoxOutput.Name = "comboBoxOutput";
			this.comboBoxOutput.Size = new System.Drawing.Size(270, 21);
			this.comboBoxOutput.TabIndex = 10;
			this.comboBoxOutput.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutput_SelectedIndexChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(154, 331);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 18;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(235, 331);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 19;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelASIO
			// 
			this.labelASIO.AutoSize = true;
			this.labelASIO.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.labelASIO.Location = new System.Drawing.Point(151, 111);
			this.labelASIO.Name = "labelASIO";
			this.labelASIO.Size = new System.Drawing.Size(98, 13);
			this.labelASIO.TabIndex = 20;
			this.labelASIO.Text = "ASIO Control Panel";
			this.labelASIO.Click += new System.EventHandler(this.labelASIO_Click);
			// 
			// PortAudioConfigEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(482, 391);
			this.Controls.Add(this.labelASIO);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textBoxOutputChannels);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBoxLatency);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.comboBoxOutput);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxInputChannels);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxSamplerate);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxInput);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxHost);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PortAudioConfigEditor";
			this.Text = "Host Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxHost;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxInput;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxSamplerate;
		private System.Windows.Forms.TextBox textBoxInputChannels;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxOutputChannels;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBoxLatency;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboBoxOutput;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelASIO;
	}
}
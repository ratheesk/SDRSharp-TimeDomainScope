namespace SDRSharp.TimeDomainScope
{
    partial class ControlPanel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_refreshTimer != null)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.waveformPanel = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.timeWindowLabel = new System.Windows.Forms.Label();
            this.timeWindowTrackBar = new System.Windows.Forms.TrackBar();
            this.timeWindowValueLabel = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.displayModeLabel = new System.Windows.Forms.Label();
            this.displayModeComboBox = new System.Windows.Forms.ComboBox();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeWindowTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // waveformPanel
            // 
            this.waveformPanel.BackColor = System.Drawing.Color.Black;
            this.waveformPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waveformPanel.Location = new System.Drawing.Point(0, 0);
            this.waveformPanel.Name = "waveformPanel";
            this.waveformPanel.Size = new System.Drawing.Size(400, 230);
            this.waveformPanel.TabIndex = 0;
            this.waveformPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.waveformPanel_Paint);
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.controlPanel.Controls.Add(this.displayModeComboBox);
            this.controlPanel.Controls.Add(this.displayModeLabel);
            this.controlPanel.Controls.Add(this.clearButton);
            this.controlPanel.Controls.Add(this.timeWindowValueLabel);
            this.controlPanel.Controls.Add(this.timeWindowTrackBar);
            this.controlPanel.Controls.Add(this.timeWindowLabel);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlPanel.Location = new System.Drawing.Point(0, 230);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(400, 70);
            this.controlPanel.TabIndex = 1;
            // 
            // timeWindowLabel
            // 
            this.timeWindowLabel.AutoSize = true;
            this.timeWindowLabel.ForeColor = System.Drawing.Color.White;
            this.timeWindowLabel.Location = new System.Drawing.Point(5, 12);
            this.timeWindowLabel.Name = "timeWindowLabel";
            this.timeWindowLabel.Size = new System.Drawing.Size(80, 15);
            this.timeWindowLabel.TabIndex = 0;
            this.timeWindowLabel.Text = "Time Window:";
            // 
            // timeWindowTrackBar
            // 
            this.timeWindowTrackBar.Location = new System.Drawing.Point(90, 5);
            this.timeWindowTrackBar.Maximum = 200;
            this.timeWindowTrackBar.Minimum = 10;
            this.timeWindowTrackBar.Name = "timeWindowTrackBar";
            this.timeWindowTrackBar.Size = new System.Drawing.Size(150, 45);
            this.timeWindowTrackBar.TabIndex = 1;
            this.timeWindowTrackBar.TickFrequency = 10;
            this.timeWindowTrackBar.Value = 50;
            this.timeWindowTrackBar.Scroll += new System.EventHandler(this.timeWindowTrackBar_Scroll);
            // 
            // timeWindowValueLabel
            // 
            this.timeWindowValueLabel.AutoSize = true;
            this.timeWindowValueLabel.ForeColor = System.Drawing.Color.Lime;
            this.timeWindowValueLabel.Location = new System.Drawing.Point(245, 12);
            this.timeWindowValueLabel.Name = "timeWindowValueLabel";
            this.timeWindowValueLabel.Size = new System.Drawing.Size(40, 15);
            this.timeWindowValueLabel.TabIndex = 2;
            this.timeWindowValueLabel.Text = "50 ms";
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(310, 8);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // displayModeLabel
            // 
            this.displayModeLabel.AutoSize = true;
            this.displayModeLabel.ForeColor = System.Drawing.Color.White;
            this.displayModeLabel.Location = new System.Drawing.Point(5, 42);
            this.displayModeLabel.Name = "displayModeLabel";
            this.displayModeLabel.Size = new System.Drawing.Size(82, 15);
            this.displayModeLabel.TabIndex = 4;
            this.displayModeLabel.Text = "Display Mode:";
            // 
            // displayModeComboBox
            // 
            this.displayModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayModeComboBox.FormattingEnabled = true;
            this.displayModeComboBox.Items.AddRange(new object[] {
            "I Component (Carrier)",
            "Q Component (Carrier)",
            "Envelope (Magnitude)",
            "Both I & Q"});
            this.displayModeComboBox.Location = new System.Drawing.Point(90, 39);
            this.displayModeComboBox.Name = "displayModeComboBox";
            this.displayModeComboBox.Size = new System.Drawing.Size(150, 23);
            this.displayModeComboBox.TabIndex = 5;
            this.displayModeComboBox.SelectedIndexChanged += new System.EventHandler(this.displayModeComboBox_SelectedIndexChanged);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.waveformPanel);
            this.Controls.Add(this.controlPanel);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(400, 300);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeWindowTrackBar)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel waveformPanel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Label timeWindowLabel;
        private System.Windows.Forms.TrackBar timeWindowTrackBar;
        private System.Windows.Forms.Label timeWindowValueLabel;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label displayModeLabel;
        private System.Windows.Forms.ComboBox displayModeComboBox;
    }
}
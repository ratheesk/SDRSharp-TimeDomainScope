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
            this.SuspendLayout();
            // 
            // waveformPanel
            // 
            this.waveformPanel.BackColor = System.Drawing.Color.Black;
            this.waveformPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waveformPanel.Location = new System.Drawing.Point(0, 0);
            this.waveformPanel.Name = "waveformPanel";
            this.waveformPanel.Size = new System.Drawing.Size(400, 300);
            this.waveformPanel.TabIndex = 0;
            this.waveformPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.waveformPanel_Paint);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.waveformPanel);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(400, 300);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel waveformPanel;
    }
}
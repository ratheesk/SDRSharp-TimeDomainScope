using System;
using System.Drawing;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.TimeDomainScope
{
    public partial class ControlPanel : UserControl
    {
        private ISharpControl _control;
        private TimeDomainProcessor _processor;
        private Timer _refreshTimer;
        private const int LEFT_MARGIN = 60;
        private const int RIGHT_MARGIN = 20;
        private const int TOP_MARGIN = 30;
        private const int BOTTOM_MARGIN = 40;

        public ControlPanel(ISharpControl control, TimeDomainProcessor processor)
        {
            _control = control;
            _processor = processor;
            InitializeComponent();

            _refreshTimer = new Timer();
            _refreshTimer.Interval = 50; // 20 FPS
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            waveformPanel.Invalidate();
        }

        private void waveformPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int width = waveformPanel.Width;
            int height = waveformPanel.Height;

            // Calculate plot area
            int plotX = LEFT_MARGIN;
            int plotY = TOP_MARGIN;
            int plotWidth = width - LEFT_MARGIN - RIGHT_MARGIN;
            int plotHeight = height - TOP_MARGIN - BOTTOM_MARGIN;

            // Clear background
            g.Clear(Color.Black);

            // Draw plot area background
            using (Brush plotBrush = new SolidBrush(Color.FromArgb(10, 10, 10)))
            {
                g.FillRectangle(plotBrush, plotX, plotY, plotWidth, plotHeight);
            }

            float[] waveformBuffer = _processor.GetAudioBuffer();
            float maxValue = _processor.GetMaxValue();

            // Draw grid with labels
            DrawGrid(g, plotX, plotY, plotWidth, plotHeight, maxValue, waveformBuffer?.Length ?? 0);

            // Draw axes
            DrawAxes(g, plotX, plotY, plotWidth, plotHeight, maxValue, waveformBuffer?.Length ?? 0);

            if (waveformBuffer == null || waveformBuffer.Length == 0)
            {
                using (Font font = new Font("Arial", 10))
                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    string msg = "Waiting for signal...";
                    SizeF msgSize = g.MeasureString(msg, font);
                    g.DrawString(msg, font, brush,
                        plotX + (plotWidth - msgSize.Width) / 2,
                        plotY + (plotHeight - msgSize.Height) / 2);
                }
                return;
            }

            // Draw waveform
            DrawWaveform(g, plotX, plotY, plotWidth, plotHeight, waveformBuffer, maxValue);

            // Draw title
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.DrawString("Time Domain Signal", titleFont, brush, plotX, 5);
            }
        }

        private void DrawGrid(Graphics g, int plotX, int plotY, int plotWidth, int plotHeight, float maxValue, int sampleCount)
        {
            using (Pen gridPen = new Pen(Color.FromArgb(40, 40, 40)))
            using (Pen majorGridPen = new Pen(Color.FromArgb(60, 60, 60)))
            {
                // Horizontal grid lines (amplitude)
                for (int i = 0; i <= 10; i++)
                {
                    int y = plotY + (plotHeight * i) / 10;
                    Pen pen = (i % 2 == 0) ? majorGridPen : gridPen;
                    g.DrawLine(pen, plotX, y, plotX + plotWidth, y);
                }

                // Vertical grid lines (time)
                for (int i = 0; i <= 10; i++)
                {
                    int x = plotX + (plotWidth * i) / 10;
                    Pen pen = (i % 2 == 0) ? majorGridPen : gridPen;
                    g.DrawLine(pen, x, plotY, x, plotY + plotHeight);
                }
            }
        }

        private void DrawAxes(Graphics g, int plotX, int plotY, int plotWidth, int plotHeight, float maxValue, int sampleCount)
        {
            using (Pen axisPen = new Pen(Color.White, 2))
            using (Pen tickPen = new Pen(Color.White, 1))
            using (Font labelFont = new Font("Arial", 8))
            using (Brush labelBrush = new SolidBrush(Color.White))
            {
                // Y-axis (amplitude)
                g.DrawLine(axisPen, plotX, plotY, plotX, plotY + plotHeight);

                // Y-axis ticks and labels
                for (int i = 0; i <= 10; i++)
                {
                    int y = plotY + (plotHeight * i) / 10;
                    g.DrawLine(tickPen, plotX - 5, y, plotX, y);

                    // Calculate amplitude value (inverted because high signal is at top)
                    float amplitude = maxValue * (10 - i) / 10.0f;
                    string label = amplitude.ToString("E2"); // Scientific notation

                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, labelBrush,
                        plotX - labelSize.Width - 8,
                        y - labelSize.Height / 2);
                }

                // Y-axis label
                using (Font axisFont = new Font("Arial", 9, FontStyle.Bold))
                {
                    string yLabel = "Amplitude";
                    SizeF labelSize = g.MeasureString(yLabel, axisFont);

                    // Rotate and draw vertical label
                    g.TranslateTransform(15, plotY + plotHeight / 2 + labelSize.Width / 2);
                    g.RotateTransform(-90);
                    g.DrawString(yLabel, axisFont, labelBrush, 0, 0);
                    g.ResetTransform();
                }

                // X-axis (time)
                g.DrawLine(axisPen, plotX, plotY + plotHeight, plotX + plotWidth, plotY + plotHeight);

                // X-axis ticks and labels
                double sampleRate = _processor.SampleRate;
                for (int i = 0; i <= 10; i++)
                {
                    int x = plotX + (plotWidth * i) / 10;
                    g.DrawLine(tickPen, x, plotY + plotHeight, x, plotY + plotHeight + 5);

                    // Calculate time value
                    double timeSamples = (sampleCount * i) / 10.0;
                    string label;

                    if (sampleRate > 0)
                    {
                        double timeMs = (timeSamples / sampleRate) * 1000.0;
                        if (timeMs < 1)
                            label = (timeMs * 1000).ToString("F1") + "μs";
                        else if (timeMs < 1000)
                            label = timeMs.ToString("F2") + "ms";
                        else
                            label = (timeMs / 1000).ToString("F2") + "s";
                    }
                    else
                    {
                        label = timeSamples.ToString("F0");
                    }

                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, labelBrush,
                        x - labelSize.Width / 2,
                        plotY + plotHeight + 8);
                }

                // X-axis label
                using (Font axisFont = new Font("Arial", 9, FontStyle.Bold))
                {
                    string xLabel = sampleRate > 0 ? "Time" : "Sample Number";
                    SizeF labelSize = g.MeasureString(xLabel, axisFont);
                    g.DrawString(xLabel, axisFont, labelBrush,
                        plotX + (plotWidth - labelSize.Width) / 2,
                        plotY + plotHeight + 25);
                }
            }
        }

        private void DrawWaveform(Graphics g, int plotX, int plotY, int plotWidth, int plotHeight,
            float[] waveformBuffer, float maxValue)
        {
            using (Pen waveformPen = new Pen(Color.Lime, 2))
            {
                int samplesPerPixel = Math.Max(1, waveformBuffer.Length / plotWidth);

                for (int x = 0; x < plotWidth - 1; x++)
                {
                    int sampleIndex = x * samplesPerPixel;
                    if (sampleIndex >= waveformBuffer.Length - 1)
                        break;

                    // Get samples
                    float sample1 = waveformBuffer[sampleIndex];
                    float sample2 = waveformBuffer[Math.Min(sampleIndex + samplesPerPixel, waveformBuffer.Length - 1)];

                    // Normalize using auto-scale
                    float normalized1 = maxValue > 0 ? sample1 / maxValue : 0;
                    float normalized2 = maxValue > 0 ? sample2 / maxValue : 0;

                    // Clamp
                    normalized1 = Math.Min(1.0f, Math.Max(0f, normalized1));
                    normalized2 = Math.Min(1.0f, Math.Max(0f, normalized2));

                    // Map to screen (inverted - high signal at top)
                    int y1 = plotY + plotHeight - (int)(normalized1 * plotHeight);
                    int y2 = plotY + plotHeight - (int)(normalized2 * plotHeight);

                    g.DrawLine(waveformPen, plotX + x, y1, plotX + x + 1, y2);
                }
            }
        }
    }
}
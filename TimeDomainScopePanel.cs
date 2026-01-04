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

        // Zoom and Pan variables
        private float _zoomLevel = 1.0f;
        private float _panOffset = 0.0f;
        private bool _isPanning = false;
        private Point _lastMousePos;
        private const float MIN_ZOOM = 1.0f;
        private const float MAX_ZOOM = 100.0f;

        public ControlPanel(ISharpControl control, TimeDomainProcessor processor)
        {
            _control = control;
            _processor = processor;
            InitializeComponent();

            // Set default display mode
            displayModeComboBox.SelectedIndex = 0; // I Component

            // Add mouse event handlers for zoom and pan
            waveformPanel.MouseWheel += WaveformPanel_MouseWheel;
            waveformPanel.MouseDown += WaveformPanel_MouseDown;
            waveformPanel.MouseMove += WaveformPanel_MouseMove;
            waveformPanel.MouseUp += WaveformPanel_MouseUp;
            waveformPanel.MouseDoubleClick += WaveformPanel_MouseDoubleClick;

            _refreshTimer = new Timer();
            _refreshTimer.Interval = 50; // 20 FPS
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void displayModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (displayModeComboBox.SelectedIndex)
            {
                case 0:
                    _processor.Mode = DisplayMode.IComponent;
                    break;
                case 1:
                    _processor.Mode = DisplayMode.QComponent;
                    break;
                case 2:
                    _processor.Mode = DisplayMode.Envelope;
                    break;
                case 3:
                    _processor.Mode = DisplayMode.Both;
                    break;
            }
            waveformPanel.Invalidate();
        }

        private void WaveformPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            // Zoom in/out with mouse wheel
            float zoomFactor = e.Delta > 0 ? 1.2f : 0.8f;
            float newZoom = _zoomLevel * zoomFactor;

            // Clamp zoom level
            newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, newZoom));

            // Adjust pan offset to zoom towards mouse position
            if (newZoom != _zoomLevel)
            {
                int plotWidth = waveformPanel.Width - LEFT_MARGIN - RIGHT_MARGIN;
                float mouseRelativePos = (float)(e.X - LEFT_MARGIN) / plotWidth;

                _panOffset = _panOffset * (newZoom / _zoomLevel);
                _zoomLevel = newZoom;

                // Clamp pan offset
                ClampPanOffset();
            }

            waveformPanel.Invalidate();
        }

        private void WaveformPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                _lastMousePos = e.Location;
                waveformPanel.Cursor = Cursors.Hand;
            }
        }

        private void WaveformPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                int plotWidth = waveformPanel.Width - LEFT_MARGIN - RIGHT_MARGIN;
                float dx = e.X - _lastMousePos.X;

                // Pan offset in normalized coordinates
                _panOffset -= (dx / plotWidth) * _zoomLevel;

                ClampPanOffset();

                _lastMousePos = e.Location;
                waveformPanel.Invalidate();
            }
        }

        private void WaveformPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = false;
                waveformPanel.Cursor = Cursors.Default;
            }
        }

        private void WaveformPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Reset zoom and pan on double-click
            if (e.Button == MouseButtons.Left)
            {
                _zoomLevel = 1.0f;
                _panOffset = 0.0f;
                waveformPanel.Invalidate();
            }
        }

        private void ClampPanOffset()
        {
            // Prevent panning beyond the signal bounds
            float maxOffset = _zoomLevel - 1.0f;
            _panOffset = Math.Max(0, Math.Min(maxOffset, _panOffset));
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

            // Draw waveform with zoom and pan
            DrawWaveform(g, plotX, plotY, plotWidth, plotHeight, waveformBuffer, maxValue);

            // Draw title and zoom info
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.DrawString("Time Domain Signal", titleFont, brush, plotX, 5);

                // Show zoom level
                string zoomInfo = $"Zoom: {_zoomLevel:F1}x";
                if (_zoomLevel > 1.0f)
                {
                    zoomInfo += " (Double-click to reset)";
                }
                SizeF infoSize = g.MeasureString(zoomInfo, titleFont);
                g.DrawString(zoomInfo, titleFont, brush, width - infoSize.Width - 10, 5);
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

                    float amplitude = maxValue * (10 - i) / 10.0f;
                    string label = amplitude.ToString("E2");

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

                    g.TranslateTransform(15, plotY + plotHeight / 2 + labelSize.Width / 2);
                    g.RotateTransform(-90);
                    g.DrawString(yLabel, axisFont, labelBrush, 0, 0);
                    g.ResetTransform();
                }

                // X-axis (time)
                g.DrawLine(axisPen, plotX, plotY + plotHeight, plotX + plotWidth, plotY + plotHeight);

                // X-axis ticks and labels with zoom consideration
                double sampleRate = _processor.SampleRate;
                int totalSamples = sampleCount;

                for (int i = 0; i <= 10; i++)
                {
                    int x = plotX + (plotWidth * i) / 10;
                    g.DrawLine(tickPen, x, plotY + plotHeight, x, plotY + plotHeight + 5);

                    // Calculate visible sample range based on zoom and pan
                    float visibleStart = _panOffset / _zoomLevel;
                    float visibleEnd = visibleStart + (1.0f / _zoomLevel);

                    float samplePos = visibleStart + ((visibleEnd - visibleStart) * i / 10.0f);
                    double timeSamples = totalSamples * samplePos;

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
            g.SetClip(new Rectangle(plotX, plotY, plotWidth, plotHeight));

            // For I/Q display, we need to handle positive and negative values differently
            bool isBipolar = (_processor.Mode == DisplayMode.IComponent ||
                              _processor.Mode == DisplayMode.QComponent ||
                              _processor.Mode == DisplayMode.Both);

            using (Pen iPen = new Pen(Color.Lime, 2))
            using (Pen qPen = new Pen(Color.Cyan, 2))
            {
                float visibleStart = _panOffset / _zoomLevel;
                float visibleEnd = visibleStart + (1.0f / _zoomLevel);

                int startSample = (int)(visibleStart * waveformBuffer.Length);
                int endSample = (int)(visibleEnd * waveformBuffer.Length);

                startSample = Math.Max(0, startSample);
                endSample = Math.Min(waveformBuffer.Length - 1, endSample);

                int visibleSamples = endSample - startSample;
                if (visibleSamples <= 0) return;

                int samplesPerPixel = Math.Max(1, visibleSamples / plotWidth);
                int centerY = plotY + plotHeight / 2;

                // Draw I component
                for (int x = 0; x < plotWidth - 1; x++)
                {
                    int sampleIndex = startSample + (x * samplesPerPixel);
                    if (sampleIndex >= endSample - 1)
                        break;

                    float sample1 = waveformBuffer[sampleIndex];
                    float sample2 = waveformBuffer[Math.Min(sampleIndex + samplesPerPixel, endSample - 1)];

                    int y1, y2;

                    if (isBipolar)
                    {
                        // Bipolar: center line is zero, positive up, negative down
                        float normalized1 = maxValue > 0 ? sample1 / maxValue : 0;
                        float normalized2 = maxValue > 0 ? sample2 / maxValue : 0;

                        normalized1 = Math.Max(-1.0f, Math.Min(1.0f, normalized1));
                        normalized2 = Math.Max(-1.0f, Math.Min(1.0f, normalized2));

                        y1 = centerY - (int)(normalized1 * (plotHeight / 2) * 0.9f);
                        y2 = centerY - (int)(normalized2 * (plotHeight / 2) * 0.9f);
                    }
                    else
                    {
                        // Unipolar (envelope): bottom is zero, top is max
                        float normalized1 = maxValue > 0 ? sample1 / maxValue : 0;
                        float normalized2 = maxValue > 0 ? sample2 / maxValue : 0;

                        normalized1 = Math.Min(1.0f, Math.Max(0f, normalized1));
                        normalized2 = Math.Min(1.0f, Math.Max(0f, normalized2));

                        y1 = plotY + plotHeight - (int)(normalized1 * plotHeight);
                        y2 = plotY + plotHeight - (int)(normalized2 * plotHeight);
                    }

                    g.DrawLine(iPen, plotX + x, y1, plotX + x + 1, y2);
                }

                // Draw Q component if Both mode
                if (_processor.Mode == DisplayMode.Both)
                {
                    float[] qBuffer = _processor.GetQBuffer();
                    if (qBuffer != null && qBuffer.Length > 0)
                    {
                        for (int x = 0; x < plotWidth - 1; x++)
                        {
                            int sampleIndex = startSample + (x * samplesPerPixel);
                            if (sampleIndex >= endSample - 1 || sampleIndex >= qBuffer.Length - 1)
                                break;

                            float sample1 = qBuffer[sampleIndex];
                            float sample2 = qBuffer[Math.Min(sampleIndex + samplesPerPixel, Math.Min(endSample - 1, qBuffer.Length - 1))];

                            float normalized1 = maxValue > 0 ? sample1 / maxValue : 0;
                            float normalized2 = maxValue > 0 ? sample2 / maxValue : 0;

                            normalized1 = Math.Max(-1.0f, Math.Min(1.0f, normalized1));
                            normalized2 = Math.Max(-1.0f, Math.Min(1.0f, normalized2));

                            int y1 = centerY - (int)(normalized1 * (plotHeight / 2) * 0.9f);
                            int y2 = centerY - (int)(normalized2 * (plotHeight / 2) * 0.9f);

                            g.DrawLine(qPen, plotX + x, y1, plotX + x + 1, y2);
                        }
                    }
                }
            }

            g.ResetClip();
        }

        private void timeWindowTrackBar_Scroll(object sender, EventArgs e)
        {
            // Update buffer size based on trackbar value (10ms to 200ms)
            int timeWindowMs = timeWindowTrackBar.Value;
            timeWindowValueLabel.Text = timeWindowMs + " ms";

            // Calculate buffer size based on sample rate
            // Assuming typical IQ sample rate of 250 kHz (adjust if needed)
            double sampleRate = _processor.SampleRate;
            if (sampleRate == 0)
                sampleRate = 250000; // Default assumption

            int bufferSize = (int)(sampleRate * timeWindowMs / 1000.0);
            _processor.BufferSize = bufferSize;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _processor.ClearBuffer();
            waveformPanel.Invalidate();
        }
    }
}
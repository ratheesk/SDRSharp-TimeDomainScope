using SDRSharp.Radio;
using System;

namespace SDRSharp.TimeDomainScope
{
    public class TimeDomainProcessor : IIQProcessor
    {
        private float[] _audioBuffer;
        private readonly object _bufferLock = new object();
        private float _maxValue = 0.001f; // Auto-scaling

        public double SampleRate { get; set; }

        public bool Enabled { get; set; }

        public unsafe void Process(Complex* buffer, int length)
        {
            if (!Enabled)
                return;

            lock (_bufferLock)
            {
                if (_audioBuffer == null || _audioBuffer.Length != length)
                {
                    _audioBuffer = new float[length];
                }

                float localMax = 0;

                // Convert IQ to magnitude (envelope) - perfect for OOK
                for (int i = 0; i < length; i++)
                {
                    float magnitude = buffer[i].Modulus();
                    _audioBuffer[i] = magnitude;

                    if (magnitude > localMax)
                        localMax = magnitude;
                }

                // Auto-scaling: track maximum value with slow decay
                if (localMax > _maxValue)
                    _maxValue = localMax;
                else
                    _maxValue *= 0.999f; // Slow decay to adapt to signal level

                // Prevent division by zero
                if (_maxValue < 0.0001f)
                    _maxValue = 0.0001f;
            }
        }

        public float[] GetAudioBuffer()
        {
            lock (_bufferLock)
            {
                if (_audioBuffer == null)
                    return null;

                float[] copy = new float[_audioBuffer.Length];
                Array.Copy(_audioBuffer, copy, _audioBuffer.Length);
                return copy;
            }
        }

        public float GetMaxValue()
        {
            return _maxValue;
        }
    }
}
using SDRSharp.Radio;
using System;
using System.Collections.Generic;

namespace SDRSharp.TimeDomainScope
{
    public class TimeDomainProcessor : IIQProcessor
    {
        private float[] _audioBuffer;
        private readonly object _bufferLock = new object();
        private float _maxValue = 0.001f;
        private Queue<float> _sampleQueue;
        private int _bufferSize = 50000; // Increased buffer size for longer time window

        public double SampleRate { get; set; }

        public bool Enabled { get; set; }

        public TimeDomainProcessor()
        {
            _sampleQueue = new Queue<float>();
        }

        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                _bufferSize = value;
                lock (_bufferLock)
                {
                    _sampleQueue.Clear();
                }
            }
        }

        public unsafe void Process(Complex* buffer, int length)
        {
            if (!Enabled)
                return;

            lock (_bufferLock)
            {
                float localMax = 0;

                // Add new samples to queue
                for (int i = 0; i < length; i++)
                {
                    float magnitude = buffer[i].Modulus();
                    _sampleQueue.Enqueue(magnitude);

                    if (magnitude > localMax)
                        localMax = magnitude;

                    // Keep queue at desired size
                    while (_sampleQueue.Count > _bufferSize)
                    {
                        _sampleQueue.Dequeue();
                    }
                }

                // Convert queue to array
                _audioBuffer = _sampleQueue.ToArray();

                // Auto-scaling: track maximum value with slow decay
                if (localMax > _maxValue)
                    _maxValue = localMax;
                else
                    _maxValue *= 0.9995f; // Slower decay for stability

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

                // Return a copy to avoid threading issues
                float[] copy = new float[_audioBuffer.Length];
                Array.Copy(_audioBuffer, copy, _audioBuffer.Length);
                return copy;
            }
        }

        public float GetMaxValue()
        {
            return _maxValue;
        }

        public void ClearBuffer()
        {
            lock (_bufferLock)
            {
                _sampleQueue.Clear();
                _audioBuffer = null;
            }
        }
    }
}
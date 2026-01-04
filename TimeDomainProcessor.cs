using SDRSharp.Radio;
using System;
using System.Collections.Generic;

namespace SDRSharp.TimeDomainScope
{
    public enum DisplayMode
    {
        Envelope,      // Magnitude (what you see now)
        IComponent,    // Real part - shows carrier
        QComponent,    // Imaginary part - shows carrier
        Both           // Both I and Q
    }

    public class TimeDomainProcessor : IIQProcessor
    {
        private float[] _audioBuffer;
        private float[] _qBuffer; // For Q component when displaying both
        private readonly object _bufferLock = new object();
        private float _maxValue = 0.001f;
        private Queue<Complex> _sampleQueue; // Changed to store Complex samples
        private int _bufferSize = 50000;
        private DisplayMode _displayMode = DisplayMode.IComponent; // Changed default

        public double SampleRate { get; set; }

        public bool Enabled { get; set; }

        public DisplayMode Mode
        {
            get => _displayMode;
            set => _displayMode = value;
        }

        public TimeDomainProcessor()
        {
            _sampleQueue = new Queue<Complex>();
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

                // Add new samples to queue (store as Complex)
                for (int i = 0; i < length; i++)
                {
                    _sampleQueue.Enqueue(buffer[i]);

                    // Track max for auto-scaling
                    float magnitude = buffer[i].Modulus();
                    if (magnitude > localMax)
                        localMax = magnitude;

                    // Keep queue at desired size
                    while (_sampleQueue.Count > _bufferSize)
                    {
                        _sampleQueue.Dequeue();
                    }
                }

                // Convert queue to arrays based on display mode
                Complex[] complexArray = _sampleQueue.ToArray();
                int count = complexArray.Length;

                if (_audioBuffer == null || _audioBuffer.Length != count)
                {
                    _audioBuffer = new float[count];
                    _qBuffer = new float[count];
                }

                switch (_displayMode)
                {
                    case DisplayMode.Envelope:
                        for (int i = 0; i < count; i++)
                        {
                            _audioBuffer[i] = complexArray[i].Modulus();
                        }
                        break;

                    case DisplayMode.IComponent:
                        for (int i = 0; i < count; i++)
                        {
                            _audioBuffer[i] = complexArray[i].Real;
                        }
                        break;

                    case DisplayMode.QComponent:
                        for (int i = 0; i < count; i++)
                        {
                            _audioBuffer[i] = complexArray[i].Imag;
                        }
                        break;

                    case DisplayMode.Both:
                        for (int i = 0; i < count; i++)
                        {
                            _audioBuffer[i] = complexArray[i].Real;
                            _qBuffer[i] = complexArray[i].Imag;
                        }
                        break;
                }

                // Auto-scaling: track maximum value with slow decay
                if (localMax > _maxValue)
                    _maxValue = localMax;
                else
                    _maxValue *= 0.9995f;

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

        public float[] GetQBuffer()
        {
            lock (_bufferLock)
            {
                if (_qBuffer == null)
                    return null;

                float[] copy = new float[_qBuffer.Length];
                Array.Copy(_qBuffer, copy, _qBuffer.Length);
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
                _qBuffer = null;
            }
        }
    }
}
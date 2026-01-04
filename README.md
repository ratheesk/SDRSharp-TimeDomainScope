# SDR# Time Domain Scope Plugin

A real-time time domain oscilloscope plugin for SDRSharp that visualizes signal waveforms with interactive zoom and pan capabilities.

## Features

- Real-time waveform visualization
- Multiple display modes (I/Q components, Envelope, Both)
- Auto-scaling amplitude with envelope detection
- Professional axes with tick marks and labels
- Interactive grid overlay
- Mouse wheel zoom (1x to 100x)
- Click-and-drag pan functionality
- Adjustable time window (10ms to 200ms)
- Optimized for viewing On-Off Keyed (OOK) signals
- Raw IQ mode for viewing actual carrier frequency

## Requirements

- SDRSharp
- .NET Framework 4.x
- Visual Studio 2022/2025 (for building)
- SDR# SDK for Plugin Developers

## Compiling Instructions

1. **Download SDR# SDK**
   - Go to [Airspy website download page](https://airspy.com/download/)
   - Download the **SDR# SDK for Plugin Developers**

2. **Clone the Repository**
```bash
   cd path/to/sdrsharp-sdk/SDRSharp
   git clone https://github.com/ratheesk/SDRSharp-TimeDomainScope.git
```
   *Note: Clone the repository inside the `sdrplugins` solution folder*

3. **Open and Build**
   - Open `SDRSharp.sln` with Visual Studio 2022/2025
   - Add the `SDRSharp.TimeDomainScope` project to the solution (if not auto-detected)
   - Set build configuration to **Release** (or Debug)
   - Build the project (Ctrl+Shift+B)

4. **Locate the DLL**
   - The compiled DLL will be in:
     - `SDRSharp.TimeDomainScope/bin/Release/SDRSharp.TimeDomainScope.dll`
     - or `SDRSharp.TimeDomainScope/bin/Debug/SDRSharp.TimeDomainScope.dll`

## Installation

1. Copy `SDRSharp.TimeDomainScope.dll` to your SDRSharp installation directory (where `SDRSharp.exe` is located)

2. Edit `Plugins.xml` in the SDRSharp directory and add the following entry:
```xml
<Plugin>
  <Type>SDRSharp.TimeDomainScope.TimeDomainScopePlugin,SDRSharp.TimeDomainScope</Type>
</Plugin>
```

3. Restart SDRSharp

## Usage

### Basic Operation

1. Open SDRSharp
2. Click on **"Time Domain Scope"** in the left panel (under **Analysis** category)
3. The plugin will display the real-time waveform of the IQ baseband signal
4. Tune to your desired frequency and the waveform will update automatically

### Display Mode Selection

Use the **Display Mode** dropdown to choose what to visualize:

- **I Component (Carrier)**: Shows the In-phase component - you'll see the actual carrier waveform
- **Q Component (Carrier)**: Shows the Quadrature component
- **Envelope (Magnitude)**: Shows only the amplitude envelope (ideal for OOK/ASK)
- **Both I & Q**: Displays both I (green) and Q (cyan) components overlaid

### Time Window Control

- Use the **Time Window** slider to adjust the visible time span (10ms to 200ms)
- Longer windows show more pulses but less detail
- Shorter windows show fewer pulses but more detail per pulse
- The time window automatically calculates buffer size based on sample rate

### Zoom and Pan Controls

- **Zoom In**: Scroll mouse wheel **up** over the waveform display
- **Zoom Out**: Scroll mouse wheel **down**
- **Pan**: Click and hold **left mouse button**, then drag to pan across the signal
- **Reset View**: **Double-click** anywhere on the display to reset zoom to 1x and center the view

*The current zoom level is displayed in the top-right corner of the display*

### Clear Buffer

- Click the **Clear** button to reset the sample buffer and start fresh

## Understanding the Display

### Why You See Lower Frequency Than Transmitted

When viewing carrier signals, you might notice the frequency appears much lower than what you transmitted. This is normal and here's why:

1. **SDR# Downconverts to Baseband**:
   - You tune SDR# to your carrier frequency (e.g., 123 kHz)
   - SDR# mixes this with a local oscillator to bring it down to **baseband (0 Hz)**
   - The IQ samples you're seeing are the **baseband signal**, not the original carrier

2. **What You're Actually Seeing**:
   - If you tuned **exactly** to 123 kHz → You see DC (0 Hz) or very low frequency
   - If you tuned **slightly off** (e.g., 123.5 kHz when signal is 123 kHz) → You see a 500 Hz beat frequency
   - The frequency offset = |Tuned Frequency - Signal Frequency|

**Example**:
- Signal transmitted at: **123.000 kHz**
- SDR# tuned to: **123.500 kHz**
- Frequency you see: **500 Hz** (the difference)

### Viewing Options

#### Option 1: View Baseband Signal (Default - Decimated IQ)
- Lower sample rate, easier to visualize
- Shows modulation clearly
- Default mode, good for most applications
- See the beat frequency by tuning slightly off-frequency

#### Option 2: View Actual Carrier (Raw IQ Mode)
To see the actual transmitted carrier frequency, you need to modify the plugin to use Raw IQ:

In `TimeDomainScopePlugin.cs`, change:
```csharp
_control.RegisterStreamHook(_processor, ProcessorType.DecimatedAndFilteredIQ);
```
to:
```csharp
_control.RegisterStreamHook(_processor, ProcessorType.RawIQ);
```

**Note**: Raw IQ mode uses the SDR's full sample rate (e.g., 2.4 MHz), so:
- You'll need to increase the time window slider range
- The display will be much faster/denser
- You'll see the actual carrier frequency (e.g., 123 kHz)

### Recommended Settings for Different Signals

#### For OOK Signals (On-Off Keying)
1. **Display Mode**: Envelope (Magnitude)
2. **SDR# Mode**: AM or RAW
3. **Filter Bandwidth**: 150-200 kHz (wider than signal)
4. **Time Window**: 50-100ms (to see multiple pulses)
5. **Squelch**: OFF (to see noise floor)

#### For Viewing Carrier Waveform
1. **Display Mode**: I Component (Carrier)
2. **Tuning**: Slightly off-frequency (1-10 Hz) for slow beat visualization
3. **Time Window**: 10-50ms
4. **Use Zoom**: Zoom in to see individual cycles

#### For FSK/PSK Analysis
1. **Display Mode**: Both I & Q
2. **Time Window**: Adjust based on symbol rate
3. **Use Zoom**: Zoom to symbol transitions

## Signal Types

The plugin can visualize:
- On-Off Keyed (OOK) signals
- Amplitude Shift Keying (ASK)
- Frequency Shift Keying (FSK) - use I&Q mode
- Phase Shift Keying (PSK) - use I&Q mode
- Pulse analysis and timing measurements
- Carrier waveforms (with proper tuning offset)
- Noise floor visualization

## Troubleshooting

- **No waveform displayed**: Check that SDRSharp is receiving a signal and the plugin is enabled
- **Signal too small**: Increase RF gain in SDRSharp or use the auto-scaling feature
- **Plugin not appearing**: Verify the DLL is in the correct directory and `Plugins.xml` is properly configured
- **Build errors**: Ensure you have the correct SDR# SDK version and all references are properly set
- **Frequency looks wrong**: Remember you're seeing baseband - tune slightly off-frequency to see beat patterns
- **Display too fast/slow**: Adjust the time window slider
- **Can't see individual cycles**: Switch to I or Q component mode and zoom in

## Performance Tips

- Reduce time window for better frame rate
- Use Envelope mode for fastest performance
- Raw IQ mode requires more processing power
- Close other plugins if experiencing lag

## License

MIT License

## Author

Rathees Koneswaran

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Acknowledgments

- Built for SDRSharp by Airspy
- Inspired by the need for real-time signal analysis tools

## Version History

See [CHANGELOG.md](CHANGELOG.md) for detailed version history.
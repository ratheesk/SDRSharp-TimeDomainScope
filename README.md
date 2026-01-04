# SDR# Time Domain Scope Plugin

A real-time time domain oscilloscope plugin for SDRSharp that visualizes signal waveforms with interactive zoom and pan capabilities.

## Features

- Real-time waveform visualization
- Auto-scaling amplitude with envelope detection
- Professional axes with tick marks and labels
- Interactive grid overlay
- Mouse wheel zoom (1x to 100x)
- Click-and-drag pan functionality
- Optimized for viewing On-Off Keyed (OOK) signals
- Envelope detection for RF signals

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

### Zoom and Pan Controls

- **Zoom In**: Scroll mouse wheel **up** over the waveform display
- **Zoom Out**: Scroll mouse wheel **down**
- **Pan**: Click and hold **left mouse button**, then drag to pan across the signal
- **Reset View**: **Double-click** anywhere on the display to reset zoom to 1x and center the view

*The current zoom level is displayed in the top-right corner of the display*

### Recommended Settings for OOK Signals

1. Set SDRSharp **Mode** to **AM** or **RAW**
2. Adjust **Filter Bandwidth** to match your signal bandwidth
3. Increase **RF Gain** if signal strength is low
4. Turn **Squelch OFF** to see the noise floor
5. Use **zoom** to focus on individual pulses or patterns

## Signal Types

The plugin displays the **envelope/magnitude** of the IQ baseband signal, making it ideal for:
- On-Off Keyed (OOK) signals
- Amplitude Shift Keying (ASK)
- Pulse analysis
- Signal timing measurements
- Noise floor visualization

## Troubleshooting

- **No waveform displayed**: Check that SDRSharp is receiving a signal and the plugin is enabled
- **Signal too small**: Increase RF gain in SDRSharp or use the auto-scaling feature
- **Plugin not appearing**: Verify the DLL is in the correct directory and `Plugins.xml` is properly configured
- **Build errors**: Ensure you have the correct SDR# SDK version and all references are properly set

## Author

Rathees Koneswaran

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Acknowledgments

- Built for SDRSharp by Airspy
- Inspired by the need for real-time signal analysis tools
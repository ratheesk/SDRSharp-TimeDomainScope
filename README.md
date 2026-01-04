# SDR# Time Domain Scope Plugin

A real-time time domain oscilloscope plugin for SDRSharp that visualizes signal waveforms.

## Features

- Real-time waveform visualization
- Auto-scaling amplitude
- Professional axes with tick marks and labels
- Grid overlay
- Optimized for viewing On-Off Keyed (OOK) signals
- Envelope detection for RF signals

## Installation

1. Build the project in Visual Studio
2. Copy `SDRSharp.TimeDomainScope.dll` to your SDRSharp installation directory
3. Add the following entry to `Plugins.xml`:

```xml
<Plugin>
  <Type>SDRSharp.TimeDomainScope.TimeDomainScopePlugin,SDRSharp.TimeDomainScope</Type>
</Plugin>
```

4. Restart SDRSharp

## Usage

1. Open SDRSharp
2. Click on "Time Domain Scope" in the left panel (under Analysis category)
3. The plugin will display the real-time waveform of the IQ baseband signal

## Requirements

- SDRSharp
- .NET Framework 4.x
- Visual Studio (for building)

## Author

Rathees Koneswaran
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Multiple display modes: I Component, Q Component, Envelope, Both I&Q
- Adjustable time window control (10ms to 200ms)
- Clear buffer button
- Queue-based rolling buffer for continuous capture
- Support for both baseband (decimated) and raw IQ modes
- Comprehensive documentation on frequency downconversion
- Display mode selector dropdown
- Auto-scaling for different signal strengths

### Changed
- Improved buffer management with configurable size
- Enhanced waveform rendering for bipolar (I/Q) signals
- Updated control panel with new controls

## [0.1.0] - 2026-01-04

### Added
- Initial implementation of time domain scope plugin
- Real-time waveform visualization
- Auto-scaling amplitude detection
- Professional axes with tick marks and labels
- Grid overlay for better readability
- Mouse wheel zoom (1x to 100x)
- Click-and-drag pan functionality
- Double-click to reset zoom
- Envelope detection for OOK signals
- Basic IQ processor implementation
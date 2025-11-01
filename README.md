SensorsToModbus

A small C# console application that runs in the system tray, collects PC sensor data, and makes it available via the Modbus RTU protocol through a serial (COM) port.

Installation & Running

Download the latest release:
Go to Releases and download the latest version.

Configure the application

Run the application once to create config.json
Right-click the tray icon and select Open Config
Modify the configuration file to match your setup

Application icon will appear in the system tray.

Tray Management

Right-click the tray icon to access:

Open Config - Opens config.json in default text editor

Reload utility - Restart the application. This will apply the configuration changes.

Show Status - Displays current connection status and active registers

Exit - Graceful application shutdown

Building from Source

Development Requirements
.NET 6.0 SDK or higher

Visual Studio 2022 or VS Code
git clone https://github.com/Sancho-M/Sensors2Modbus
cd SensorsToModbus

Libralies Used

NModbus - Modbus protocol implementation
LibreHardwareMonitorLib - Hardware sensor data collection

License

This project is licensed under the MIT License - see the LICENSE file for details.

Third-Party Licenses

This software uses the following third-party libraries:
LibreHardwareMonitorLib - Licensed under Mozilla Public License 2.0
NModbus - Licensed under MIT License


Known Limitations

Windows only

Modbus RTU only (RS485/RS232)

Administrator privileges recommended for sensor access


ATTENTION: Restart the application after changing the configuration.

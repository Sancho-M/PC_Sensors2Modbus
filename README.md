<div style="text-align: center; width: 100%;">
  <div style="display: inline-block; text-align: center;">
    <img src="https://i.ibb.co/ccfsmvw8/image.png" width="80" height="80" style="display: block; margin: 0 auto 5px auto;">
  </div>
</div>

## About

A small C# console application that runs in the system tray, collects PC sensor data, and exposes it via the Modbus RTU protocol through a serial (COM) port.

---

## Installation & Running

### Download the latest release
Go to **Releases** and download the latest version.

### Configure the application

1. Run the application once to create `config.json`
2. Right-click the tray icon and select **Open Config**
3. Modify the configuration file to match your setup

> The application icon will appear in the system tray.

---

## Tray Management

Right-click the tray icon to access:

- **Open Config** — Opens `config.json` in default text editor  
- **Reload utility** — Restart the application (applies config changes)  
- **Show Status** — Displays current connection status and active registers  
- **Exit** — Graceful application shutdown  

---

## Building from Source

### Requirements

- .NET 6.0 SDK or higher  
- Visual Studio 2022 or VS Code

### Clone and Build

```bash
git clone https://github.com/Sancho-M/Sensors2Modbus
cd SensorsToModbus
```

---

## Libraries Used

- **NModbus** — Modbus protocol implementation  
- **LibreHardwareMonitorLib** — Hardware sensor data collection  

---

## License

This project is licensed under the **MIT License** — see the `LICENSE` file for details.

---

## Third-Party Licenses

This software uses the following third-party libraries:

| Library | License |
|--------|--------|
| LibreHardwareMonitorLib | Mozilla Public License 2.0 |
| NModbus | MIT License |

---

## Known Limitations

- Windows only  
- Modbus RTU only (RS485/RS232)  
- Administrator privileges recommended for sensor access  

---

### ⚠️ Attention

Restart the application after changing the configuration.

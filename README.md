<div align="center">
  <img src="https://i.ibb.co/ccfsmvw8/image.png" width="80" height="80">
</div>

## About

A lightweight C# console application that runs in the system tray, collects PC sensor data, and transmits it using Modbus RTU over serial (COM) port.

---

## Installation & Running

### Download the latest release
Go to **Releases** and download the latest version.

### Configure the application

1. Run the application once to create `config.json`
2. Right-click the tray icon and select **Open Config**
3. Modify the configuration file to match your setup

> The application icon will appear in the system tray.

Typical Configuration Example:

```json
{
  "COM_PORT": "COM4",
  "BOUDRATE": 19200,
  "SLAVE_ID": 1,
  "REG_0_CPU_TEMPERATURE": true,
  "REG_1_CPU_LOAD": true,
  "REG_2_GPU_TEMPERATURE": true,
  "REG_3_GPU_LOAD": true,
  "REG_4_RAM_LOAD": true
}
```

## Tray Management

Right-click the tray icon to access:

- **Show Status** — Displays current connection status
- **Open console** - Opens the console with logs that show successful and failed connection attempts and their timestamps
- **Close console** - Hides the console from view
- **Open Config** — Opens `Sensors2Modbus.json` in default text editor  
- **Reload utility** — Restart the application (applies config changes)  
- **Exit** — Graceful application shutdown  

---

## Building from Source

### Requirements

- .NET 6.0 SDK or higher  
- Visual Studio 2022 or VS Code

### Clone and Build

```bash
git clone https://github.com/Sancho-M/Sensors2Modbus
cd Sensors2Modbus
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

This software uses the following third-party libraries — See the `THIRD-PARTY-NOTICES.txt` file for details.

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

Please add the .exe file to Windows Defender exclusions. Antivirus programs often flag LibreHardwareMonitorlib.

After making changes to the configuration, 
click the "reload utility" option in the tray.

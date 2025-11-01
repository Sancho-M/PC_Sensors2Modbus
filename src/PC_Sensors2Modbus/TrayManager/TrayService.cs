using SensorsManager;
class TrayService : IDisposable
{
    private NotifyIcon _trayIcon;
    private readonly ModbusService modbusTray;
    private readonly SensorsToRegistersWritter linkerTray;
    private readonly SettingsManager _settingsManager;

    private bool _isConsoleVisible = true;
    private bool _firstStatusReceived = false;

    private DateTime _lastNotificationTime = DateTime.MinValue;
    private string message = "Status not received";

    public TrayService(ModbusService modbus, SensorsToRegistersWritter linker, SettingsManager settings)
    {
        modbusTray = modbus;
        linkerTray = linker;
        _settingsManager = settings;
        modbusTray.StatusChanged += OnModbusStatusChanged;
    }

    public async Task InitializeTrayIconAsync()
    {
        var trayMenu = new ContextMenuStrip();

        trayMenu.Items.Add("Show console", null, (s, e) => ShowConsole());
        trayMenu.Items.Add("Hide console", null, (s, e) => HideConsole());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("Status", null, (s, e) => ShowStatus());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("Reload utility", null, async (s, e) => await RestartModbusAsync());
        trayMenu.Items.Add("Open configuration file", null, (s, e) => _settingsManager.OpenJson());
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add("Exit", null, (s, e) => ExitApplication());
 

        _trayIcon = new NotifyIcon
        {
            Icon = CreateTrayIcon(modbusTray.IsWorking),
            Text = "Modbus Utility",
            ContextMenuStrip = trayMenu,
            Visible = true
        };

        _trayIcon.DoubleClick += (s, e) => ToggleConsole();
        _trayIcon.Click += (s, e) =>
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                ShowStatus();
            }
        };

        Application.ApplicationExit += (s, e) =>
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
        };

        await Task.Yield();
    }

    private async Task RestartModbusAsync()
    {
        //ShowNotification("Restart", "In progress...");
        UpdateTrayIcon(false);

        //await modbusTray.StopAsync();

        Application.Restart();

        await modbusTray.StartAsync();
        await linkerTray.RunAsync();
    }

    private void OnModbusStatusChanged(bool oldValue, bool newValue)
    {
        if (oldValue == newValue && _firstStatusReceived)
            return;
        _firstStatusReceived = true;

        if (newValue)
        {
            if (modbusTray.errorTime == DateTime.MinValue)
                message = "Running. No errors detected!";
            else
                message = $"Running. Last error date: {modbusTray.errorTime}";
        }
        else
        {
            message = $"NOT WORKING!";
        }

        if ((DateTime.Now - _lastNotificationTime).TotalSeconds >= 0.2)
        {
            _lastNotificationTime = DateTime.Now;
            UpdateTrayIcon(newValue);
            ShowNotification("Modbus Status", message);
        }
    }

    private void UpdateTrayIcon(bool isWorking)
    {
        try
        {
            _trayIcon.Icon = CreateTrayIcon(isWorking);
        }
        catch
        {
            _trayIcon.Icon = SystemIcons.Information;
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    extern static bool DestroyIcon(IntPtr handle);

    private Icon CreateTrayIcon(bool isWorking)
    {
        try
        {
            using (var bitmap = new Bitmap(16, 16))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Color backgroundColor = isWorking ? Color.Green : Color.Red;

                graphics.Clear(backgroundColor);
                graphics.DrawString("M", new Font("Arial", 8, FontStyle.Bold),
                                  Brushes.White, new PointF(1, 2));

                IntPtr hIcon = bitmap.GetHicon();
                try
                {
                    var icon = (Icon)Icon.FromHandle(hIcon).Clone();
                    return icon;
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
        }
        catch
        {
            return SystemIcons.Information;
        }
    }

    private void ShowNotification(string title, string message)
    {
        try
        {
            _trayIcon?.ShowBalloonTip(500, title, message, ToolTipIcon.Info);
        }
        catch { }
    }

    private void ShowStatus()
    {
        ShowNotification("Status", message);
    }

    private void ShowConsole()
    {
        try
        {
            var consoleWindow = ConsoleHelper.GetConsoleWindow();
            ConsoleHelper.ShowWindow(consoleWindow, ConsoleHelper.SW_SHOW);
            ConsoleHelper.SetForegroundWindow(consoleWindow);
            _isConsoleVisible = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to show console: {ex.Message}");
        }
    }

    private void HideConsole()
    {
        try
        {
            var consoleWindow = ConsoleHelper.GetConsoleWindow();
            ConsoleHelper.ShowWindow(consoleWindow, ConsoleHelper.SW_HIDE);
            _isConsoleVisible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to hide console: {ex.Message}");
        }
    }

    private void ToggleConsole()
    {
        if (_isConsoleVisible) HideConsole();
        else ShowConsole();
    }

    private void ExitApplication()
    {
        Application.Exit();
    }

    public void Dispose()
    {
        if (modbusTray != null)
        {
            modbusTray.StatusChanged -= OnModbusStatusChanged;
        }

        _trayIcon?.Dispose();
    }
}

internal static class ConsoleHelper
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;
}
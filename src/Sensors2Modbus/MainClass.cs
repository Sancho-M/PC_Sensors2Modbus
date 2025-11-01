using SensorsManager;

class MainClass
{
    [STAThread]
    static async Task Main()
    {
        ConsoleHelper.ShowWindow(ConsoleHelper.GetConsoleWindow(), ConsoleHelper.SW_HIDE);
        Application.SetCompatibleTextRenderingDefault(false);

        using CancellationTokenSource cts = new();

        SensorsService sensors = new(cts.Token);
        ModbusService modbus = new(cts.Token);
        SensorsToRegistersWritter linker = new(sensors, modbus, cts.Token);
        SettingsManager settings = new SettingsManager();
        TrayService tray = new(modbus, linker, settings);

        _ = tray.InitializeTrayIconAsync();
        _ = modbus.StartAsync();
        _ = linker.RunAsync();

        settings.ReloadSettings();

        Application.ApplicationExit += async (s, e) =>
        {
            cts.Cancel();
            await Task.WhenAll(modbus.StopAsync(), linker.StopAsync());
            tray.Dispose();  
            sensors.Dispose();
        };

        Application.Run();

    }//.
}
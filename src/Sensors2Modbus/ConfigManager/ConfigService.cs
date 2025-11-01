using System.Text.Json;

public class AppSettings
{
    public string COM_PORT { get; set; } = "COM3";
    public int BOUDRATE { get; set; } = 9600;
    public byte SLAVE_ID { get; set; } = 1;
    public bool REG_0_CPU_TEMPERATURE { get; set; } = true;
    public bool REG_1_CPU_LOAD { get; set; } = true;
    public bool REG_2_GPU_TEMPERATURE { get; set; } = true;
    public bool REG_3_GPU_LOAD { get; set; } = true;
    public bool REG_4_RAM_LOAD { get; set; } = true;
}

public class SettingsManager
{
    private const string SettingsFile = "PC_Sensors2Modbus_Сonfig.json";
    string fullPath = Path.GetFullPath(SettingsFile);
    public void OpenJson()
    {
        try
        {
            if (File.Exists(fullPath))
            {
                System.Diagnostics.Process.Start("notepad.exe", fullPath);
            }
            else
            {
                ChangeConfigStatus("Сonfiguration file not found.");
            }
        }
        catch (Exception ex)
        {
            ChangeConfigStatus($"Unable to open the configuration file.. {ex.Message}");
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(SettingsFile, json);
    }

    public AppSettings ReloadSettings()
    {
        var defaultSettings = new AppSettings();

        if (!File.Exists(SettingsFile))
        {
            ChangeConfigStatus("Cannot open the configuration file. Creating a new one");
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        try
        {
            string json = File.ReadAllText(SettingsFile);
            using var doc = JsonDocument.Parse(json);

            var jsonProps = doc.RootElement.EnumerateObject()
                                           .Select(p => p.Name)
                                           .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var classProps = typeof(AppSettings).GetProperties()
                                                .Select(p => p.Name)
                                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!classProps.All(p => jsonProps.Contains(p)))
            {
                ChangeConfigStatus("The JSON is missing parameters. Recreating from template.");
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            return JsonSerializer.Deserialize<AppSettings>(json) ?? defaultSettings;
        }
        catch
        {
            ChangeConfigStatus("JSON read error. Recreating from template");
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }
    private void ChangeConfigStatus(string status)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(status);
        Console.ResetColor();

    }

}


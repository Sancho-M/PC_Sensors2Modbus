using LibreHardwareMonitor.Hardware;

namespace SensorsManager;
internal class SensorsService : ISensorsService, IDisposable
{
    ushort[] data = new ushort[5];

    private readonly Computer _computer;

    public SensorsService(CancellationToken token)
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsStorageEnabled = true
        };

    }

    public ushort[]? GetValues()
    {
        try
        {
            _computer.Open();
            _computer.Accept(new UpdateVisitor());

            foreach (var hardware in _computer.Hardware)
            {
                foreach (var sensor in hardware.Sensors)
                {
                    if (!sensor.Value.HasValue)
                        continue;

                    switch (sensor.SensorType)
                    {
                        case SensorType.Temperature:
                            if (sensor.Name == "Core (Tctl/Tdie)" || sensor.Name == "Core Average")
                            {
                                data[0] = (ushort)sensor.Value.Value; // Температура CPU
                            }
                            else if (sensor.Name == "GPU Core")
                            {
                                data[2] = (ushort)sensor.Value.Value; // Температура GPU
                            }
                            break;

                        case SensorType.Load:
                            if (sensor.Name == "CPU Total")
                            {
                                data[1] = (ushort)sensor.Value.Value; // Нагрузка CPU %
                            }
                            else if (sensor.Name == "Memory")
                            {
                                data[4] = (ushort)sensor.Value.Value; // Нагрузка RAM %
                            }
                            else if (sensor.Name == "GPU Core")
                            {
                                data[3] = (ushort)sensor.Value.Value; // Нагрузка GPU %
                            }
                            break;
                    }
                }
            }
            return data;
        }
        catch (OperationCanceledException)
        {
            Dispose();
        }

        finally
        {
            Dispose();
        }

        return null;
    }

    public void Dispose()
    {
        _computer.Close();
    }

    private class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer) => computer.Traverse(this);

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (var subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}

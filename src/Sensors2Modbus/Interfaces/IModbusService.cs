using NModbus.Data;

public interface IModbusService
{
    UInt16 DALAY_BETWEEN_REQUEST { get; set; }

    public event Action<bool, bool> StatusChanged; // oldValue, newValue
    bool IsWorking { get; }
    public void UpdateRegisters(ushort[] reg);
    Task StartAsync();
    Task StopAsync();
    SlaveDataStore GetDataStore();
}

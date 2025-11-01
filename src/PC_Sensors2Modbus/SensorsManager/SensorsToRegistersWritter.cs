
namespace SensorsManager;
class SensorsToRegistersWritter
{
    private readonly ISensorsService _sensors;
    private readonly IModbusService _modbus;
    private readonly CancellationToken _parentToken;

    private Task? _updaterTask;
    private CancellationTokenSource? _internalCts;

    public SensorsToRegistersWritter(ISensorsService sensors, IModbusService modbus, CancellationToken parentToken)
    {
        _sensors = sensors;
        _modbus = modbus;
        _parentToken = parentToken;
    }

    public async Task RunAsync()
    {
        await StopAsync();

        _internalCts = CancellationTokenSource.CreateLinkedTokenSource(_parentToken);
        _updaterTask = UpdateLoopAsync(_internalCts.Token);
        await Task.Yield();
    }

    private async Task UpdateLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (!_modbus.IsWorking)
                {
                    await Task.Delay(1000, token);
                    continue;
                }

                var buffer = _sensors.GetValues();

                if (buffer != null)
                {
                    try
                    {
                        _modbus.UpdateRegisters(buffer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Write reg error: {ex.Message}");
                    }
                }

                await Task.Delay(_modbus.DALAY_BETWEEN_REQUEST, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update loop error: {ex.Message}");
                await Task.Delay(1000, token);
            }
        }
    }

    public async Task StopAsync()
    {
        if (_internalCts == null)
            return;

        try
        {
            _internalCts.Cancel();

            if (_updaterTask != null)
            {
                try { await _updaterTask; }
                catch (OperationCanceledException) { }
            }
        }
        finally
        {
            _internalCts.Dispose();
            _internalCts = null;
            _updaterTask = null;
        }
    }
}

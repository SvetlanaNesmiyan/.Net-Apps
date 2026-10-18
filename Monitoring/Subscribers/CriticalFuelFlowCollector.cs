using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Monitoring.Subscribers;

/// <summary>
/// Індивідуальний підписник (Варіант 7): накопичує критичні показники
/// витрати палива в окремий список для подальшого аналізу.
/// </summary>
public class CriticalFuelFlowCollector
{
    public List<TelemetryReading> CriticalReadings { get; } = new();

    public void OnStatusEvaluated(TelemetryReading reading, SafetyStatus status)
    {
        if (status == SafetyStatus.Critical)
        {
            CriticalReadings.Add(reading);
        }
    }
}

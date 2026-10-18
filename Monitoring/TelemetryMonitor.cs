using IoTTelemetryLab.Evaluation;
using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Monitoring;

/// <summary>
/// Джерело подій. Аналізує показники та повідомляє підписників.
/// </summary>
public class TelemetryMonitor
{
    private readonly SensorEvaluatorRegistry _registry;
    private readonly SensorType _sensorType;

    // Подія: зовні можна використовувати лише += та -=, але не Invoke.
    public event Action<TelemetryReading, SafetyStatus>? OnStatusEvaluated;

    public TelemetryMonitor(SensorEvaluatorRegistry registry, SensorType sensorType)
    {
        _registry = registry;
        _sensorType = sensorType;
    }

    public void Process(TelemetryReading reading)
    {
        var status = _registry.Evaluate(_sensorType, reading.Value);
        OnStatusEvaluated?.Invoke(reading, status);
    }
}

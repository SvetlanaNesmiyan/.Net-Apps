using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Evaluation;

/// <summary>
/// Стратегія оцінки витрати палива (л/год). Пороги для варіанту 7.
/// </summary>
public class FuelFlowEvaluator : ISensorEvaluator
{
    // Поріг: Критичний > 15, Попередження > 10
    public SafetyStatus Evaluate(double value) => value switch
    {
        > 15.0 => SafetyStatus.Critical,
        > 10.0 => SafetyStatus.Warning,
        _ => SafetyStatus.Normal
    };
}

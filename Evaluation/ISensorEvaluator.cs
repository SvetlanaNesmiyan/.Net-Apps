using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Evaluation;

/// <summary>
/// Контракт для стратегії оцінки стану датчика.
/// </summary>
public interface ISensorEvaluator
{
    /// <summary>
    /// Оцінює числове значення та повертає статус безпеки.
    /// </summary>
    SafetyStatus Evaluate(double value);
}

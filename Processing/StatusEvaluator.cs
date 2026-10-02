using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Processing;

/// <summary>
/// Рівень небезпеки показу датчика.
/// </summary>
public enum SafetyStatus
{
    Normal,
    Warning,
    Critical
}

/// <summary>
/// Оцінює рівень небезпеки показу для заданого типу датчика
/// через вкладені switch-вирази з реляційними та логічними патернами.
/// </summary>
public static class StatusEvaluator
{
    /// <summary>
    /// Повертає <see cref="SafetyStatus"/> для показу заданого типу датчика.
    /// </summary>
    /// <param name="type">Тип датчика.</param>
    /// <param name="value">Значення показу.</param>
    public static SafetyStatus Evaluate(SensorType type, double value) => type switch
    {
        SensorType.Temperature => value switch
        {
            < 0 or > 80 => SafetyStatus.Critical,
            >= 60 and <= 80 => SafetyStatus.Warning,
            _ => SafetyStatus.Normal
        },
        SensorType.Vibration => value switch
        {
            > 5.0 => SafetyStatus.Critical,
            > 2.5 => SafetyStatus.Warning,
            _ => SafetyStatus.Normal
        },
        SensorType.Voltage => value switch
        {
            < 200 or > 240 => SafetyStatus.Critical,
            < 210 or > 230 => SafetyStatus.Warning,
            _ => SafetyStatus.Normal
        },
        // Варіант 7: тиск (кПа). Нормальний атмосферний тиск ≈ 101.3 кПа.
        // <80 або >120 кПа — критичне відхилення (ризик для обладнання);
        // <95 або >110 кПа — попередження (відхилення ~10%).
        SensorType.Pressure => value switch
        {
            < 80 or > 120 => SafetyStatus.Critical,
            < 95 or > 110 => SafetyStatus.Warning,
            _ => SafetyStatus.Normal
        },
        _ => SafetyStatus.Normal
    };
}

using IoTTelemetryLab.Models;
using IoTTelemetryLab.Evaluation;

namespace IoTTelemetryLab.Processing;

/// <summary>
/// Аналітичні LINQ-обчислення над очищеним потоком телеметрії.
/// </summary>
public static class TelemetryAnalytics
{
    /// <summary>
    /// Підраховує кількість показів, рівень небезпеки яких дорівнює target,
    /// використовуючи переданий реєстр стратегій.
    /// </summary>
    public static int CountByStatus(
        IEnumerable<TelemetryReading> readings,
        SensorType type,
        SafetyStatus target,
        SensorEvaluatorRegistry registry)
    {
        return readings.Count(r => registry.Evaluate(type, r.Value) == target);
    }

    /// <summary>
    /// Обчислює ковзне середнє за вікном windowSize.
    /// </summary>
    public static IEnumerable<double> MovingAverage(
        IEnumerable<TelemetryReading> readings,
        int windowSize)
    {
        var values = readings.Select(r => r.Value).ToList();

        for (int i = 0; i <= values.Count - windowSize; i++)
        {
            yield return values.Skip(i).Take(windowSize).Average();
        }
    }
}
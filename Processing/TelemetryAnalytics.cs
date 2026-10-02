using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Processing;

/// <summary>
/// Аналітичні LINQ-обчислення над очищеним потоком телеметрії.
/// </summary>
public static class TelemetryAnalytics
{
    /// <summary>
    /// Підраховує кількість показів, рівень небезпеки яких дорівнює <paramref name="target"/>.
    /// Використано LINQ-метод Count із предикатом.
    /// </summary>
    public static int CountByStatus(
        IEnumerable<TelemetryReading> readings,
        SensorType type,
        SafetyStatus target)
    {
        return readings.Count(r => StatusEvaluator.Evaluate(type, r.Value) == target);
    }

    /// <summary>
    /// Обчислює ковзне середнє за вікном <paramref name="windowSize"/>.
    /// Використано Select, Skip, Take, Average та ітератор yield return
    /// (відкладене виконання).
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

    /// <summary>
    /// Індивідуальне завдання варіанта 7 (тиск):
    /// повертає кількість Normal-показів та середнє значення Critical-показів.
    /// Використано LINQ-методи Where, Select, Count, Average, Any.
    /// </summary>
    /// <param name="readings">Очищений потік показів.</param>
    /// <returns>Кортеж (NormalCount, CriticalAverage).</returns>
    public static (int NormalCount, double CriticalAverage) PressureStats(
        IEnumerable<TelemetryReading> readings)
    {
        var list = readings.ToList();

        int normalCount = list.Count(r =>
            StatusEvaluator.Evaluate(SensorType.Pressure, r.Value) == SafetyStatus.Normal);

        var criticalValues = list
            .Where(r => StatusEvaluator.Evaluate(SensorType.Pressure, r.Value) == SafetyStatus.Critical)
            .Select(r => r.Value);

        double criticalAverage = criticalValues.Any() ? criticalValues.Average() : 0.0;

        return (normalCount, criticalAverage);
    }
}

using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Processing;

/// <summary>
/// Очищення «сирих» показів від апаратних помилок.
/// </summary>
public static class TelemetryCleaner
{
    private const double ErrorMarker = -999.0;

    /// <summary>
    /// Видаляє покази, значення яких є NaN або маркером апаратної помилки.
    /// </summary>
    /// <param name="raw">Потік «сирих» показів.</param>
    /// <returns>Відфільтрований потік (відкладене виконання LINQ).</returns>
    public static IEnumerable<TelemetryReading> RemoveInvalid(
        IEnumerable<TelemetryReading> raw)
    {
        return raw.Where(r =>
            !double.IsNaN(r.Value) &&
            r.Value != ErrorMarker);
    }
}

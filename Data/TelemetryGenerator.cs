using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Data;

public static class TelemetryGenerator
{
    /// <summary>
    /// Генерує пакет «сирих» показів від заданого датчика.
    /// Приблизно 10% показів — апаратні помилки.
    /// </summary>
    /// <param name="deviceId">Ідентифікатор пристрою.</param>
    /// <param name="count">Кількість показів у пакеті.</param>
    /// <param name="minValue">Нижня межа коректних значень.</param>
    /// <param name="maxValue">Верхня межа коректних значень.</param>
    /// <returns>Список «сирих» показів.</returns>
    public static List<TelemetryReading> GenerateRawBatch(
        string deviceId,
        int count,
        double minValue,
        double maxValue)
    {
        var random = new Random();
        var readings = new List<TelemetryReading>();
        var baseTime = DateTime.UtcNow;

        for (int i = 0; i < count; i++)
        {
            double value = random.NextDouble() < 0.1
                ? (random.Next(2) == 0 ? -999.0 : double.NaN)
                : Math.Round(minValue + random.NextDouble() * (maxValue - minValue), 2);

            readings.Add(new TelemetryReading(
                DeviceId: deviceId,
                Value: value,
                Timestamp: baseTime.AddSeconds(i * 5)));
        }

        return readings;
    }
}

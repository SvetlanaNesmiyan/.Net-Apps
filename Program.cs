using IoTTelemetryLab.Data;
using IoTTelemetryLab.Models;
using IoTTelemetryLab.Processing;

const string deviceId = "sensor-07";
const SensorType sensorType = SensorType.Pressure;

// Діапазон 60–140 кПа перекриває і Warning (<95, >110), і Critical (<80, >120)
// з запасом в обидва боки — щоб гарантовано отримати покази всіх трьох рівнів.
var rawData = TelemetryGenerator.GenerateRawBatch(
    deviceId, count: 50, minValue: 60, maxValue: 140);

Console.WriteLine($"Отримано показів: {rawData.Count}");

var cleaned = TelemetryCleaner.RemoveInvalid(rawData).ToList();
Console.WriteLine($"Після очищення: {cleaned.Count}");

int criticalCount = TelemetryAnalytics.CountByStatus(cleaned, sensorType, SafetyStatus.Critical);
int warningCount  = TelemetryAnalytics.CountByStatus(cleaned, sensorType, SafetyStatus.Warning);
Console.WriteLine($"Критичних показів: {criticalCount}, попереджень: {warningCount}");

var averages = TelemetryAnalytics.MovingAverage(cleaned, windowSize: 5).ToList();
Console.WriteLine("Ковзне середнє (перші 3 вікна): " +
    string.Join(", ", averages.Take(3).Select(a => a.ToString("F2"))));

var (normalCount, criticalAvg) = TelemetryAnalytics.PressureStats(cleaned);
Console.WriteLine($"Normal-показів тиску: {normalCount}");
Console.WriteLine($"Середнє Critical-показів тиску: {criticalAvg:F2} кПа");

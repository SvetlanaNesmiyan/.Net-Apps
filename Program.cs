using IoTTelemetryLab.Data;
using IoTTelemetryLab.Evaluation;
using IoTTelemetryLab.Monitoring;
using IoTTelemetryLab.Monitoring.Subscribers;
using IoTTelemetryLab.Models;
using IoTTelemetryLab.Processing;

var registry = EvaluatorSetup.CreateDefaultRegistry();

var temperatureMonitor = new TelemetryMonitor(registry, SensorType.Temperature);
var vibrationMonitor = new TelemetryMonitor(registry, SensorType.Vibration);
var voltageMonitor = new TelemetryMonitor(registry, SensorType.Voltage);
var fuelFlowMonitor = new TelemetryMonitor(registry, SensorType.FuelFlow);

var criticalCounter = new CriticalCounterSubscriber();
var fuelFlowCollector = new CriticalFuelFlowCollector();

temperatureMonitor.OnStatusEvaluated += ConsoleAlertSubscriber.OnStatusEvaluated;
temperatureMonitor.OnStatusEvaluated += criticalCounter.OnStatusEvaluated;
vibrationMonitor.OnStatusEvaluated += ConsoleAlertSubscriber.OnStatusEvaluated;
vibrationMonitor.OnStatusEvaluated += criticalCounter.OnStatusEvaluated;
voltageMonitor.OnStatusEvaluated += ConsoleAlertSubscriber.OnStatusEvaluated;
voltageMonitor.OnStatusEvaluated += criticalCounter.OnStatusEvaluated;
fuelFlowMonitor.OnStatusEvaluated += ConsoleAlertSubscriber.OnStatusEvaluated;
fuelFlowMonitor.OnStatusEvaluated += criticalCounter.OnStatusEvaluated;
fuelFlowMonitor.OnStatusEvaluated += fuelFlowCollector.OnStatusEvaluated;

var allReadings = new List<(TelemetryReading Reading, SensorType Type)>();
allReadings.AddRange(TelemetryGenerator.GenerateRawBatch("TEMP-001", 250, -20, 120).Select(r => (r, SensorType.Temperature)));
allReadings.AddRange(TelemetryGenerator.GenerateRawBatch("VIB-001", 250, 0, 10).Select(r => (r, SensorType.Vibration)));
allReadings.AddRange(TelemetryGenerator.GenerateRawBatch("VOLT-001", 250, 0, 5).Select(r => (r, SensorType.Voltage)));
allReadings.AddRange(TelemetryGenerator.GenerateRawBatch("FUEL-001", 250, 0, 100).Select(r => (r, SensorType.FuelFlow)));

foreach (var (reading, type) in allReadings)
{
    switch (type)
    {
        case SensorType.Temperature:
            temperatureMonitor.Process(reading);
            break;
        case SensorType.Vibration:
            vibrationMonitor.Process(reading);
            break;
        case SensorType.Voltage:
            voltageMonitor.Process(reading);
            break;
        case SensorType.FuelFlow:
            fuelFlowMonitor.Process(reading);
            break;
    }
}

var cleanReadings = allReadings
    .Select(x => x.Reading)
    .ToList();
cleanReadings = TelemetryCleaner.RemoveInvalid(cleanReadings).ToList();

var cleanWithType = allReadings
    .Where(x => !double.IsNaN(x.Reading.Value) && x.Reading.Value != -999.0)
    .ToList();

Console.WriteLine("\n--- Analytics ---");
Console.WriteLine($"Critical Temperature: {TelemetryAnalytics.CountByStatus(cleanWithType.Where(x => x.Type == SensorType.Temperature).Select(x => x.Reading), SensorType.Temperature, SafetyStatus.Critical, registry)}");
Console.WriteLine($"Critical Vibration: {TelemetryAnalytics.CountByStatus(cleanWithType.Where(x => x.Type == SensorType.Vibration).Select(x => x.Reading), SensorType.Vibration, SafetyStatus.Critical, registry)}");
Console.WriteLine($"Critical Voltage: {TelemetryAnalytics.CountByStatus(cleanWithType.Where(x => x.Type == SensorType.Voltage).Select(x => x.Reading), SensorType.Voltage, SafetyStatus.Critical, registry)}");
Console.WriteLine($"Critical FuelFlow: {TelemetryAnalytics.CountByStatus(cleanWithType.Where(x => x.Type == SensorType.FuelFlow).Select(x => x.Reading), SensorType.FuelFlow, SafetyStatus.Critical, registry)}");

Console.WriteLine($"\nTotal Critical Count: {criticalCounter.CriticalCount}");
Console.WriteLine($"FuelFlow Critical Readings Collected: {fuelFlowCollector.CriticalReadings.Count}");

Console.WriteLine("\nMoving Average (Temperature, window=5):");
var tempReadings = cleanWithType.Where(x => x.Type == SensorType.Temperature).Select(x => x.Reading);
foreach (var avg in TelemetryAnalytics.MovingAverage(tempReadings, 5).Take(10))
{
    Console.WriteLine($"{avg:F2}");
}

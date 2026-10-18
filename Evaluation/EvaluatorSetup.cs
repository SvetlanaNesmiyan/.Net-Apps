using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Evaluation;

/// <summary>
/// Фабрика для створення реєстру зі стандартними стратегіями.
/// </summary>
public static class EvaluatorSetup
{
    public static SensorEvaluatorRegistry CreateDefaultRegistry()
    {
        var registry = new SensorEvaluatorRegistry();

        registry.Register(SensorType.Temperature, new TemperatureEvaluator());
        registry.Register(SensorType.Vibration, new VibrationEvaluator());
        registry.Register(SensorType.Voltage, new VoltageEvaluator());
        // Реєстрація стратегії для варіанту 7 (завдання А)
        registry.Register(SensorType.FuelFlow, new FuelFlowEvaluator());

        return registry;
    }
}

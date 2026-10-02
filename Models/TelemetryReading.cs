namespace IoTTelemetryLab.Models;

/// <summary>
/// Незмінна модель одного показу IoT-датчика.
/// readonly record struct — легкий value-тип для масових короткоживучих вимірювань.
/// </summary>
public readonly record struct TelemetryReading(
    string DeviceId,
    double Value,
    DateTime Timestamp);

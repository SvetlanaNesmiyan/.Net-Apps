using IoTTelemetryLab.Data;
using IoTTelemetryLab.Evaluation;
using IoTTelemetryLab.Models;
using IoTTelemetryLab.Monitoring;
using IoTTelemetryLab.Monitoring.Subscribers;
using IoTTelemetryLab.Processing;

const string deviceId = "fuel-sensor-07";
const SensorType sensorType = SensorType.FuelFlow; // ВАР: 7

// 1. Створюємо реєстр і реєструємо стратегію (Завдання А)
var registry = EvaluatorSetup.CreateDefaultRegistry();

// 2. Демонстрація Завдання Б: реєструємо ДРУГУ стратегію для того самого типу за допомогою Func-лямбди.
// Обидва оцінювачі (клас FuelFlowEvaluator та лямбда) дають однаковий результат.
// Для основного циклу можна використовувати будь-яку з них. Залишимо зареєстровану за замовчуванням.
// registry.Register(SensorType.FuelFlow, value => value switch {
//     > 15.0 => SafetyStatus.Critical,
//     > 10.0 => SafetyStatus.Warning,
//     _ => SafetyStatus.Normal
// });

// 3. Створюємо монітор
var monitor = new TelemetryMonitor(registry, sensorType);

// 4. Створюємо підписників
var criticalCounter = new CriticalCounterSubscriber();
var fuelCollector = new CriticalFuelFlowCollector(); // Наш індивідуальний підписник

// 5. Підписуємося на подію (multicast)
monitor.OnStatusEvaluated += ConsoleAlertSubscriber.OnStatusEvaluated;
monitor.OnStatusEvaluated += criticalCounter.OnStatusEvaluated;
monitor.OnStatusEvaluated += fuelCollector.OnStatusEvaluated; // Подключаем вариант 7

// 6. Генеруємо дані
// Для датчика витрати палива (Critical > 15, Warning > 10) задаємо діапазон 5–20 л/год,
// щоб у вибірку потрапляли всі три стани.
var rawData = TelemetryGenerator.GenerateRawBatch(deviceId, count: 50, minValue: 5, maxValue: 20);
var cleaned = TelemetryCleaner.RemoveInvalid(rawData).ToList();

// 7. Обробляємо дані (подія спрацьовує для кожного показання)
foreach (var reading in cleaned)
{
    monitor.Process(reading);
}

// 8. Виводимо підсумкову аналітику
Console.WriteLine($"\nВсего критических состояний (счетчик): {criticalCounter.CriticalCount}");

// Демонстрація роботи індивідуального передплатника (Варіант 7)
Console.WriteLine($"\n--- Анализ критических показаний расхода топлива (Вариант 7) ---");
Console.WriteLine($"Собрано критических показаний: {fuelCollector.CriticalReadings.Count}");
if (fuelCollector.CriticalReadings.Any())
{
    var values = fuelCollector.CriticalReadings.Select(r => r.Value).ToList();
    Console.WriteLine($"Мин. значение: {values.Min():F2} л/ч");
    Console.WriteLine($"Макс. значение: {values.Max():F2} л/ч");
    Console.WriteLine($"Среднее значение: {values.Average():F2} л/ч");
    Console.WriteLine($"Последнее критическое показание: {fuelCollector.CriticalReadings.Last().Value:F2} л/ч " +
                      $"({fuelCollector.CriticalReadings.Last().Timestamp:HH:mm:ss})");
}

// Демонстрація того, що клас-оцінювач і Func-оцінювач є взаємозамінними (Завдання Б, п. 2)
Console.WriteLine("\n--- Проверка взаимозаменяемости оценщиков (Задание Б) ---");
double testValue = 16.0;
var classResult = new FuelFlowEvaluator().Evaluate(testValue);
var funcResult = registry.Evaluate(SensorType.FuelFlow, testValue); // використовує зареєстрований Func
Console.WriteLine($"Значение {testValue} -> Результат класса: {classResult}, Результат Func: {funcResult}. Совпадают: {classResult == funcResult}");

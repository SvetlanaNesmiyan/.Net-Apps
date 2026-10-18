using IoTTelemetryLab.Models;

namespace IoTTelemetryLab.Evaluation;

/// <summary>
/// Реєстр стратегій оцінювання. Замінює вираз switch із ЛР1.
/// </summary>
public class SensorEvaluatorRegistry
{
    private readonly Dictionary<SensorType, ISensorEvaluator> _evaluators = new();

    /// <summary>
    /// Реєструє стратегію-об’єкт для вказаного типу датчика.
    /// </summary>
    public void Register(SensorType type, ISensorEvaluator evaluator)
    {
        _evaluators[type] = evaluator;
    }

    /// <summary>
    /// Реєструє делегат Func як стратегію (завдання Б).
    /// </summary>
    public void Register(SensorType type, Func<double, SafetyStatus> evaluatorFunc)
    {
        _evaluators[type] = new FuncEvaluatorAdapter(evaluatorFunc);
    }

    /// <summary>
    /// Визначає значення для вказаного типу датчика.
    /// </summary>
    public SafetyStatus Evaluate(SensorType type, double value)
    {
        if (!_evaluators.TryGetValue(type, out var evaluator))
        {
            throw new InvalidOperationException($"Немає зареєстрованого оцінювача для типу {type}");
        }
        return evaluator.Evaluate(value);
    }

    /// <summary>
    /// Внутрішній адаптер, що перетворює делегат Func на ISensorEvaluator.
    /// </summary>
    private sealed class FuncEvaluatorAdapter : ISensorEvaluator
    {
        private readonly Func<double, SafetyStatus> _func;
        public FuncEvaluatorAdapter(Func<double, SafetyStatus> func) => _func = func;
        public SafetyStatus Evaluate(double value) => _func(value);
    }
}

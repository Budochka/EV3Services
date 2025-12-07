using System.Collections.Generic;

namespace VideoAudioProcessor;

/// <summary>
/// Adaptive noise floor learning that adjusts thresholds based on environment.
/// </summary>
public class AdaptiveNoiseFloor
{
    private readonly Queue<double> _recentEnergy = new();
    private readonly int _windowSize;
    private double _learnedNoiseFloor;
    private int _updateCounter = 0;
    private const int UpdateInterval = 10; // Recalculate median every 10 updates

    public AdaptiveNoiseFloor(int windowSize, double initialNoiseFloor = 200.0)
    {
        _windowSize = windowSize;
        _learnedNoiseFloor = initialNoiseFloor;
    }

    public void Update(double rmsEnergy)
    {
        _recentEnergy.Enqueue(rmsEnergy);

        if (_recentEnergy.Count > _windowSize)
        {
            _recentEnergy.Dequeue();
        }

        // Update noise floor as median of recent low-energy samples
        // This represents the baseline noise level in the environment
        // Only recalculate periodically to avoid expensive sorting on every frame
        if (_recentEnergy.Count >= _windowSize && ++_updateCounter >= UpdateInterval)
        {
            _updateCounter = 0;
            var sorted = new List<double>(_recentEnergy);
            sorted.Sort();
            _learnedNoiseFloor = sorted[sorted.Count / 2];
        }
    }

    public double GetThreshold(double multiplier)
    {
        return _learnedNoiseFloor * multiplier;
    }

    public double CurrentNoiseFloor => _learnedNoiseFloor;
}


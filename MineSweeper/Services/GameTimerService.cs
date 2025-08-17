using System.Threading;

/// <summary>
/// Provides a simple timer service that can be started, stopped, and reset.
/// Raises events whenever the time changes.
/// Uses a cancellation token to support clean stop of the timer loop.
/// </summary>
internal class GameTimerService
{
    private bool _isRunning = false;
    public bool IsRunning => _isRunning;

    private int _time = 0;
    public int time => _time;

    private CancellationTokenSource? _cts;

    /// <summary>
    /// Event triggered whenever the timer value increments.
    /// </summary>
    public event Action<int>? OnTimeChanged;

    /// <summary>
    /// Starts the timer loop asynchronously. Increments time every second.
    /// </summary>
    public async void StartTimer()
    {
        if (_isRunning) return;

        _isRunning = true;
        _cts = new CancellationTokenSource();

        try
        {
            while (_isRunning)
            {
                await Task.Delay(1000, _cts.Token);
                if (!_isRunning) break;

                _time++;
                OnTimeChanged?.Invoke(_time);
            }
        }
        catch (TaskCanceledException)
        {
            // Swallow cancellation
        }
    }

    /// <summary>
    /// Stops the timer loop immediately and cancels pending delays.
    /// </summary>
    public void StopTimer()
    {
        _isRunning = false;
        _cts?.Cancel();
        _cts = null;
    }

    /// <summary>
    /// Resets the timer value back to zero and notifies listeners.
    /// </summary>
    public void ResetTimer()
    {
        _time = 0;
        OnTimeChanged?.Invoke(_time);
    }
}

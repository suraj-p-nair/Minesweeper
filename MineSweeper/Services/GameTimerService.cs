using System.Threading;

internal class GameTimerService
{
    private bool _isRunning = false;
    public bool IsRunning => _isRunning;
    private int _time = 0;

    public int time => _time;
    private CancellationTokenSource? _cts;

    public event Action<int>? OnTimeChanged;

    public async void StartTimer()
    {
        if (_isRunning) return;

        _isRunning = true;
        _cts = new CancellationTokenSource(); // always create new CTS on start

        try
        {
            while (_isRunning)
            {
                await Task.Delay(1000, _cts.Token); // cancellable delay
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

    public void StopTimer()
    {
        _isRunning = false;
        _cts?.Cancel();   // cancel current delay instantly
        _cts = null;      // release old CTS so StartTimer can make a new one
    }

    public void ResetTimer()
    {
        _time = 0;
        OnTimeChanged?.Invoke(_time);
    }
}

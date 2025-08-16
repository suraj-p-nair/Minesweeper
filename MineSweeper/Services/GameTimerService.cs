internal class GameTimerService
{
    private bool _isRunning = false;
    private int _time = 0;

    public event Action<int>? OnTimeChanged; // notify UI helper

    public async void StartTimer()
    {
        if (_isRunning) return;
        _isRunning = true;

        while (_isRunning)
        {
            await Task.Delay(1000);
            _time++;
            OnTimeChanged?.Invoke(_time);
        }
    }

    public void ResetTimer()
    {
        _time = 0;
        OnTimeChanged?.Invoke(_time);
    }
}

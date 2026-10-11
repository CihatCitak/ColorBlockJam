using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.GameplaySystem
{
    internal class LevelTimer
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private readonly Action<int> _ticked;
        private readonly Action _timeUp;

        private CancellationTokenSource _running;
        private int _secondsLeft;

        public LevelTimer(Action<int> ticked, Action timeUp)
        {
            _ticked = ticked;
            _timeUp = timeUp;
        }

        public bool IsRunning => _running != null;

        public void SetTime(int seconds)
        {
            _secondsLeft = seconds;
            _ticked(_secondsLeft);
        }

        public void AddTime(int seconds)
        {
            _secondsLeft += seconds;
            _ticked(_secondsLeft);
        }

        public void Resume()
        {
            if (IsRunning)
                return;

            _running = new CancellationTokenSource();
            RunAsync(_running.Token).Forget();
        }

        public void Pause()
        {
            _running?.Cancel();
            _running?.Dispose();
            _running = null;
        }

        private async UniTaskVoid RunAsync(CancellationToken token)
        {
            while (_secondsLeft > 0)
            {
                if (await UniTask.Delay(TickInterval, cancellationToken: token).SuppressCancellationThrow())
                    return;

                _secondsLeft--;
                _ticked(_secondsLeft);
            }

            Pause();
            _timeUp();
        }
    }
}

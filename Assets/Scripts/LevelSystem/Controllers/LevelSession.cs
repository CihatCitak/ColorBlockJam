using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.PlayerSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    internal class LevelSession : MonoBehaviour
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private CancellationTokenSource _timer;
        private int _blocksLeft;
        private int _secondsLeft;
        private bool _isFinished;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelReadyEvent>(OnLevelReady);
            EventBus.Subscribe<BlockExitedEvent>(OnBlockExited);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelReadyEvent>(OnLevelReady);
            EventBus.Unsubscribe<BlockExitedEvent>(OnBlockExited);
            StopTimer();
        }

        private void OnLevelReady(LevelReadyEvent e)
        {
            _blocksLeft = e.BlockCount;
            _secondsLeft = e.TimeLimit;
            EventBus.Fire(new LevelTimeChangedEvent(_secondsLeft));

            _timer = new CancellationTokenSource();
            RunTimerAsync(_timer.Token).Forget();
        }

        private async UniTaskVoid RunTimerAsync(CancellationToken token)
        {
            while (_secondsLeft > 0)
            {
                if (await UniTask.Delay(TickInterval, cancellationToken: token).SuppressCancellationThrow())
                    return;

                _secondsLeft--;
                EventBus.Fire(new LevelTimeChangedEvent(_secondsLeft));
            }

            Finish();
            EventBus.Fire(new LevelFailedEvent());
        }

        private void OnBlockExited(BlockExitedEvent e)
        {
            if (_isFinished)
                return;

            _blocksLeft--;
            if (_blocksLeft > 0)
                return;

            Finish();
            ServiceLocator.Get<IPlayerDataStore>().CompleteLevel();
            EventBus.Fire(new LevelCompletedEvent());
        }

        private void Finish()
        {
            _isFinished = true;
            StopTimer();
            EventBus.Fire(new InputLockChangedEvent(true));
        }

        private void StopTimer()
        {
            _timer?.Cancel();
            _timer?.Dispose();
            _timer = null;
        }
    }
}

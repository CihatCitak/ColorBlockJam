using System;
using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.LevelSystem;
using Game.PlayerSystem;
using Game.PopupSystem;
using Game.SceneSystem;
using Game.UI.Common;
using UnityEngine;

namespace Game.GameplaySystem
{
    internal class GameplayManager : MonoBehaviour
    {
        [SerializeField] private GameplayConfig _config;
        [SerializeField] private PlayOnConfig _playOnConfig;
        [SerializeField] private GameplayHudView _hud;

        private LevelTimer _timer;
        private IPopupService _popups;
        private ICoinWallet _wallet;
        private ILives _lives;
        private IPlayerSettings _settings;
        private int _levelNumber;
        private int _blocksLeft;
        private bool _hasStarted;
        private int _playOnCount;

        private PlayOnTier CurrentPlayOn
        {
            get
            {
                var tiers = _playOnConfig.Tiers;
                return tiers[Mathf.Min(_playOnCount, tiers.Count - 1)];
            }
        }

        private void Awake()
        {
            _timer = new LevelTimer(_hud.SetTime, OnTimeUp);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<BlockExitedEvent>(OnBlockExited);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<BlockExitedEvent>(OnBlockExited);
            EventBus.Unsubscribe<ScreenTouchedEvent>(OnScreenTouched);
            _timer.Pause();
        }

        private void Start()
        {
            StartLevelAsync().Forget();
        }

        private async UniTaskVoid StartLevelAsync()
        {
            var loader = ServiceLocator.Get<ILevelLoader>();
            var creator = ServiceLocator.Get<ILevelCreator>();

            await UniTask.WaitUntil(() => loader.IsInitialized && creator.IsInitialized);

            _popups = ServiceLocator.Get<IPopupService>();
            _wallet = ServiceLocator.Get<ICoinWallet>();
            _lives = ServiceLocator.Get<ILives>();
            _settings = ServiceLocator.Get<IPlayerSettings>();
            _levelNumber = ServiceLocator.Get<IPlayerLevelProgress>().CurrentLevel;

            var data = await loader.LoadAsync(_levelNumber);
            creator.Create(data);

            _blocksLeft = data.Blocks.Count;
            _hud.Show(_levelNumber, data.TimeLimit, OnRestartClicked, OnPauseClicked);
            _hud.SetPlayControlsInteractable(false);
            _timer.SetTime(data.TimeLimit);
            EventBus.Subscribe<ScreenTouchedEvent>(OnScreenTouched);

            EventBus.Fire(new LevelReadyEvent());
        }

        private void OnScreenTouched(ScreenTouchedEvent e)
        {
            EventBus.Unsubscribe<ScreenTouchedEvent>(OnScreenTouched);
            _hasStarted = true;
            _hud.SetPlayControlsInteractable(true);
            _timer.Resume();
        }

        private void OnBlockExited(BlockExitedEvent e)
        {
            _blocksLeft--;
            if (_timer.IsRunning)
                TryWin();
        }

        private void OnTimeUp()
        {
            if (TryWin())
                return;

            Pause();
            var playOn = CurrentPlayOn;
            _popups.Show<OutOfTimePopupView>().Setup(playOn.Seconds, playOn.Cost, _wallet.CanAfford(playOn.Cost), OnContinueWithCoins, ContinueAfterTimeUp, OnOutOfTimeClosed);
        }

        private bool TryWin()
        {
            if (_blocksLeft > 0)
                return false;

            Pause();
            _hud.SetInteractable(false);
            ServiceLocator.Get<IPlayerLevelProgress>().CompleteLevel();
            ShowLevelCompleteAsync().Forget();
            return true;
        }

        private async UniTaskVoid ShowLevelCompleteAsync()
        {
            var delay = TimeSpan.FromSeconds(_config.LevelCompleteDelay);
            if (await UniTask.Delay(delay, cancellationToken: destroyCancellationToken).SuppressCancellationThrow())
                return;

            var reward = _config.LevelReward;
            _popups.Show<LevelCompletePopupView>().Setup(reward, reward * _config.RewardMultiplier, ClaimReward);
        }

        private void OnRestartClicked()
        {
            Pause();
            _popups.Show<LoseLifePopupView>().SetupRetry(_levelNumber, LoseLifeAndRetry, CloseAndResume);
        }

        private void OnPauseClicked()
        {
            Pause();
            _popups.Show<SettingsPopupView>().Setup(_settings.IsOn, _settings.Set, OnHomeClicked, CloseAndResume);
        }

        private void OnHomeClicked()
        {
            if (!_hasStarted)
            {
                LoadMain();
                return;
            }

            _popups.Show<LoseLifePopupView>().SetupHome(_levelNumber, LoseLifeAndGoHome, _popups.Close);
        }

        private void OnContinueWithCoins()
        {
            if (_wallet.TrySpend(CurrentPlayOn.Cost))
                ContinueAfterTimeUp();
        }

        private void ContinueAfterTimeUp()
        {
            _timer.AddTime(CurrentPlayOn.Seconds);
            _playOnCount++;
            CloseAndResume();
        }

        private void OnOutOfTimeClosed()
        {
            _popups.Close();
            _popups.Show<LoseLifePopupView>().SetupRetry(_levelNumber, LoseLifeAndRetry, LoseLifeAndGoHome);
        }

        private void CloseAndResume()
        {
            _popups.Close();
            Resume();
        }

        private void Pause()
        {
            _timer.Pause();
            EventBus.Fire(new InputLockChangedEvent(true));
        }

        private void Resume()
        {
            if (TryWin())
                return;

            EventBus.Fire(new InputLockChangedEvent(false));
            if (_hasStarted)
                _timer.Resume();
        }

        private void ClaimReward(int amount)
        {
            _wallet.Add(amount);
            LoadMain();
        }

        private void LoseLifeAndRetry()
        {
            _lives.LoseLife();
            if (_lives.Count == 0)
            {
                LoadMain();
                return;
            }

            _popups.CloseAll();
            ServiceLocator.Get<ISceneLoader>().LoadGameplayAsync().Forget();
        }

        private void LoseLifeAndGoHome()
        {
            _lives.LoseLife();
            LoadMain();
        }

        private void LoadMain()
        {
            _popups.CloseAll();
            ServiceLocator.Get<ISceneLoader>().LoadMainAsync().Forget();
        }
    }
}

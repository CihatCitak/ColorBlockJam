using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.PlayerSystem;
using Game.PopupSystem;
using Game.SceneSystem;
using Game.UI.Common;
using UnityEngine;

namespace Game.MainMenuSystem
{
    internal class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private MainMenuConfig _config;
        [SerializeField] private MainMenuView _view;
        [SerializeField] private BottomBarView _bottomBar;
        [SerializeField] private GameObject[] _screens;
        [SerializeField] private int _homeTab = 1;

        private IPopupService _popups;
        private IPlayerSettings _settings;
        private ILives _lives;
        private ICoinWallet _wallet;

        private void Start()
        {
            _popups = ServiceLocator.Get<IPopupService>();
            _settings = ServiceLocator.Get<IPlayerSettings>();
            _lives = ServiceLocator.Get<ILives>();
            _wallet = ServiceLocator.Get<ICoinWallet>();

            _view.Show(ServiceLocator.Get<IPlayerLevelProgress>().CurrentLevel, OnPlayClicked, OnSettingsClicked, OnLivesClicked);
            _bottomBar.Show(SelectTab);
            SelectTab(_homeTab);
        }

        private void SelectTab(int index)
        {
            for (var i = 0; i < _screens.Length; i++)
                _screens[i].SetActive(i == index);

            _bottomBar.Select(index);
        }

        private void OnPlayClicked()
        {
            if (_lives.Count == 0)
            {
                ShowRefillLives();
                return;
            }

            _view.SetInteractable(false);
            ServiceLocator.Get<ISceneLoader>().LoadGameplayAsync().Forget();
        }

        private void OnSettingsClicked()
        {
            _popups.Show<SettingsPopupView>().Setup(_settings.IsOn, _settings.Set, null, _popups.Close);
        }

        private void OnLivesClicked()
        {
            if (!_lives.IsFull)
                ShowRefillLives();
        }

        private void ShowRefillLives()
        {
            var cost = _config.RefillLivesCost;
            _popups.Show<RefillLivesPopupView>().Setup(_config.AdLives, cost, _wallet.CanAfford(cost), OnAdLivesClicked, OnRefillLivesClicked, _popups.Close);
        }

        private void OnAdLivesClicked()
        {
            _lives.Add(_config.AdLives);
            _popups.Close();
        }

        private void OnRefillLivesClicked()
        {
            if (!_wallet.TrySpend(_config.RefillLivesCost))
                return;

            _lives.Fill();
            _popups.Close();
        }
    }
}

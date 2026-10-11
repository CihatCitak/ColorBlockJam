using System.Globalization;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.PlayerSystem;
using TMPro;
using UnityEngine;

namespace Game.UI.Common
{
    internal class CoinsView : MonoBehaviour
    {
        private const int Thousand = 1000;
        private const float Hundredths = 100f;

        private static readonly NumberFormatInfo CommaDecimal = new() { NumberDecimalSeparator = "," };

        [SerializeField] private TMP_Text _amountText;

        private void OnEnable()
        {
            EventBus.Subscribe<CoinsChangedEvent>(OnCoinsChanged);
            Show(ServiceLocator.Get<ICoinWallet>().Coins);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        private void OnCoinsChanged(CoinsChangedEvent e)
        {
            Show(e.Coins);
        }

        private void Show(int coins)
        {
            if (coins < Thousand)
            {
                _amountText.SetText("{0}", coins);
                return;
            }

            var thousands = Mathf.Floor(coins * Hundredths / Thousand) / Hundredths;
            _amountText.SetText(thousands.ToString("0.##", CommaDecimal) + "k");
        }
    }
}

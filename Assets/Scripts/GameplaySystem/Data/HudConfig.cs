using UnityEngine;

namespace Game.GameplaySystem
{
    [CreateAssetMenu(menuName = "Game/Hud Config", fileName = "HudConfig")]
    internal class HudConfig : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _disabledControlAlpha = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _pulseFromTimeRatio = 0.5f;
        [SerializeField, Min(1f)] private float _pulseScale = 1.25f;
        [SerializeField, Min(0f)] private float _pulseDuration = 0.4f;
        [SerializeField, Min(0)] private int _warningFromSeconds = 10;
        [SerializeField, Range(0f, 1f)] private float _warningAlpha = 0.85f;
        [SerializeField, Min(0f)] private float _warningFlashDuration = 0.5f;

        public float DisabledControlAlpha => _disabledControlAlpha;
        public float PulseFromTimeRatio => _pulseFromTimeRatio;
        public float PulseScale => _pulseScale;
        public float PulseDuration => _pulseDuration;
        public int WarningFromSeconds => _warningFromSeconds;
        public float WarningAlpha => _warningAlpha;
        public float WarningFlashDuration => _warningFlashDuration;
    }
}

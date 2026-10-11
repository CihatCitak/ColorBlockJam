using UnityEngine;

namespace Game.GameplaySystem
{
    [CreateAssetMenu(menuName = "Game/Gameplay Config", fileName = "GameplayConfig")]
    internal class GameplayConfig : ScriptableObject
    {
        [SerializeField, Min(0)] private int _levelReward = 40;
        [SerializeField, Min(1)] private int _rewardMultiplier = 2;
        [SerializeField, Min(0f)] private float _levelCompleteDelay = 0.6f;

        public int LevelReward => _levelReward;
        public int RewardMultiplier => _rewardMultiplier;
        public float LevelCompleteDelay => _levelCompleteDelay;
    }
}

using UnityEngine;

namespace Game.ObstacleSystem
{
    [CreateAssetMenu(menuName = "Game/Obstacle Config", fileName = "ObstacleConfig")]
    internal class ObstacleConfig : ScriptableObject
    {
        [SerializeField] private GameObject _obstaclePrefab;

        public GameObject ObstaclePrefab => _obstaclePrefab;
    }
}

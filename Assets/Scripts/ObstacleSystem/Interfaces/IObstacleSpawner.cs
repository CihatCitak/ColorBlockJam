using UnityEngine;

namespace Game.ObstacleSystem
{
    public interface IObstacleSpawner
    {
        void Spawn(Vector2Int cell);
    }
}

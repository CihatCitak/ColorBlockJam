using System.Collections.Generic;
using Game.BlockSystem;
using Game.ExitSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    [CreateAssetMenu(menuName = "Game/Level Data", fileName = "Level_")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private Vector2Int _gridSize = new(4, 5);
        [SerializeField, Min(1)] private int _timeLimit = 60;
        [SerializeField] private List<Vector2Int> _holes = new();
        [SerializeField] private List<Vector2Int> _obstacles = new();
        [SerializeField] private List<ExitPlacement> _exits = new();
        [SerializeField] private List<BlockPlacement> _blocks = new();

        public Vector2Int GridSize => _gridSize;
        public int TimeLimit => _timeLimit;
        public IReadOnlyList<Vector2Int> Holes => _holes;
        public IReadOnlyList<Vector2Int> Obstacles => _obstacles;
        public IReadOnlyList<ExitPlacement> Exits => _exits;
        public IReadOnlyList<BlockPlacement> Blocks => _blocks;
    }
}

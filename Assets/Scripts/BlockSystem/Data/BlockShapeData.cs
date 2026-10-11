using System.Collections.Generic;
using UnityEngine;

namespace Game.BlockSystem
{
    [CreateAssetMenu(menuName = "Game/Block Shape", fileName = "BlockShape_")]
    public class BlockShapeData : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> _cells = new() { Vector2Int.zero };
        [SerializeField] private Block _prefab;

        public IReadOnlyList<Vector2Int> Cells => _cells;
        internal Block Prefab => _prefab;
    }
}

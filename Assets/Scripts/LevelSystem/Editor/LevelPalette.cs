using System.Collections.Generic;
using Game.BlockSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    [CreateAssetMenu(menuName = "Game/Level Palette", fileName = "LevelPalette")]
    internal class LevelPalette : ScriptableObject
    {
        [SerializeField] private List<BlockShapeData> _shapes = new();
        [SerializeField] private List<BlockColorData> _colors = new();

        public IReadOnlyList<BlockShapeData> Shapes => _shapes;
        public IReadOnlyList<BlockColorData> Colors => _colors;
    }
}

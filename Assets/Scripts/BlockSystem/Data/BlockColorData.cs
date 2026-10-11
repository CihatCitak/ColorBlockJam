using UnityEngine;

namespace Game.BlockSystem
{
    [CreateAssetMenu(menuName = "Game/Block Color", fileName = "BlockColor_")]
    public class BlockColorData : ScriptableObject
    {
        [SerializeField] private Color _color = Color.white;

        public Color Color => _color;
    }
}

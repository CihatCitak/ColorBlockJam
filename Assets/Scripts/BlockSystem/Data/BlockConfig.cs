using DG.Tweening;
using UnityEngine;

namespace Game.BlockSystem
{
    [CreateAssetMenu(menuName = "Game/Block Config", fileName = "BlockConfig")]
    internal class BlockConfig : ScriptableObject
    {
        [SerializeField, Range(2f, 60f)] private float _moveSpeed = 20f;
        [SerializeField, Range(0f, 1f)] private float _snapDuration = 0.15f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [SerializeField, Range(1f, 30f)] private float _exitSpeed = 6f;
        [SerializeField] private Ease _exitEase = Ease.Linear;
        [SerializeField, Range(1, 20)] private int _crushParticlesPerPart = 4;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)] private float _outlineWidth = 4f;

        public float MoveSpeed => _moveSpeed;
        public float SnapDuration => _snapDuration;
        public Ease SnapEase => _snapEase;
        public float ExitSpeed => _exitSpeed;
        public Ease ExitEase => _exitEase;
        public int CrushParticlesPerPart => _crushParticlesPerPart;
        public Color OutlineColor => _outlineColor;
        public float OutlineWidth => _outlineWidth;
    }
}

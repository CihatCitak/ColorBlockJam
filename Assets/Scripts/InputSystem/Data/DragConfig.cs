using UnityEngine;

namespace Game.InputSystem
{
    [CreateAssetMenu(menuName = "Game/Drag Config", fileName = "DragConfig")]
    internal class DragConfig : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _touchRadius = 0.15f;

        public float TouchRadius => _touchRadius;
    }
}

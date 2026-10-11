using UnityEngine;

namespace Game.CameraSystem
{
    [CreateAssetMenu(menuName = "Game/Camera Config", fileName = "CameraConfig")]
    internal class CameraConfig : ScriptableObject
    {
        [SerializeField, Range(10f, 90f)] private float _fieldOfView = 35f;
        [SerializeField, Range(0f, 60f)] private float _pitch = 15f;
        [SerializeField, Range(0f, 0.4f)] private float _padding = 0.06f;

        public float FieldOfView => _fieldOfView;
        public float Pitch => _pitch;
        public float Padding => _padding;
    }
}

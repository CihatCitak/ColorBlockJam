using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.GridSystem;
using UnityEngine;

namespace Game.CameraSystem
{
    [RequireComponent(typeof(Camera))]
    internal class CameraFitter : MonoBehaviour
    {
        private const int SearchSteps = 20;

        [SerializeField] private CameraConfig _config;
        [SerializeField] private Camera _camera;

        private readonly Vector3[] _corners = new Vector3[4];

        private void OnEnable()
        {
            EventBus.Subscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnLevelReady(LevelReadyEvent e)
        {
            Fit(ServiceLocator.Get<IGrid>().Area);
        }

        private void Fit(Rect area)
        {
            _corners[0] = new Vector3(area.xMin, area.yMin);
            _corners[1] = new Vector3(area.xMin, area.yMax);
            _corners[2] = new Vector3(area.xMax, area.yMin);
            _corners[3] = new Vector3(area.xMax, area.yMax);

            _camera.fieldOfView = _config.FieldOfView;
            transform.rotation = Quaternion.Euler(-_config.Pitch, 0f, 0f);

            var near = _camera.nearClipPlane;
            var far = _camera.farClipPlane;
            for (var i = 0; i < SearchSteps; i++)
            {
                var distance = (near + far) * 0.5f;
                PlaceAt(area.center, distance);
                if (Fits())
                    far = distance;
                else
                    near = distance;
            }

            PlaceAt(area.center, far);
        }

        private void PlaceAt(Vector2 center, float distance)
        {
            transform.position = (Vector3)center - transform.forward * distance;
        }

        private bool Fits()
        {
            var min = _config.Padding;
            var max = 1f - _config.Padding;
            foreach (var corner in _corners)
            {
                var point = _camera.WorldToViewportPoint(corner);
                if (point.z <= 0f || point.x < min || point.x > max || point.y < min || point.y > max)
                    return false;
            }

            return true;
        }
    }
}

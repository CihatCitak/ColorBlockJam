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
        [SerializeField] private RectTransform _boardArea;

        private readonly Vector3[] _corners = new Vector3[4];
        private readonly Vector3[] _areaCorners = new Vector3[4];

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
            Fit(ServiceLocator.Get<IGrid>().Area, ViewportArea());
        }

        private Rect ViewportArea()
        {
            _boardArea.GetWorldCorners(_areaCorners);
            var min = _camera.ScreenToViewportPoint(_areaCorners[0]);
            var max = _camera.ScreenToViewportPoint(_areaCorners[2]);
            return Rect.MinMaxRect(min.x + _config.Padding, min.y + _config.Padding, max.x - _config.Padding, max.y - _config.Padding);
        }

        private void Fit(Rect area, Rect viewport)
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
                PlaceAt(area.center, distance, viewport.center);
                if (Fits(viewport))
                    far = distance;
                else
                    near = distance;
            }

            PlaceAt(area.center, far, viewport.center);
        }

        private void PlaceAt(Vector2 center, float distance, Vector2 viewportCenter)
        {
            var halfHeight = distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var halfWidth = halfHeight * _camera.aspect;
            var shift = transform.right * ((viewportCenter.x - 0.5f) * 2f * halfWidth)
                + transform.up * ((viewportCenter.y - 0.5f) * 2f * halfHeight);
            transform.position = (Vector3)center - transform.forward * distance - shift;
        }

        private bool Fits(Rect viewport)
        {
            foreach (var corner in _corners)
            {
                var point = _camera.WorldToViewportPoint(corner);
                if (point.z <= 0f || !viewport.Contains(point))
                    return false;
            }

            return true;
        }
    }
}

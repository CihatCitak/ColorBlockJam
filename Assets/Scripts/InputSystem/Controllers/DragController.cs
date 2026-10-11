using Game.Core.Events;
using Game.Events;
using Lean.Touch;
using UnityEngine;

namespace Game.InputSystem
{
    internal class DragController : MonoBehaviour
    {
        private const int MaxTouchHits = 8;

        [SerializeField] private DragConfig _config;
        [SerializeField] private Camera _camera;

        private readonly Plane _boardPlane = new(Vector3.forward, Vector3.zero);
        private readonly Collider2D[] _hits = new Collider2D[MaxTouchHits];

        private LeanFinger _finger;
        private IDraggable _dragged;
        private Vector2 _grabOffset;
        private bool _isLocked;

        private void OnEnable()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUpdate += OnFingerUpdate;
            LeanTouch.OnFingerUp += OnFingerUp;
            EventBus.Subscribe<InputLockChangedEvent>(OnInputLockChanged);
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUpdate -= OnFingerUpdate;
            LeanTouch.OnFingerUp -= OnFingerUp;
            EventBus.Unsubscribe<InputLockChangedEvent>(OnInputLockChanged);
        }

        private void OnInputLockChanged(InputLockChangedEvent e)
        {
            _isLocked = e.IsLocked;
            if (_isLocked && _dragged != null)
                Release();
        }

        private void OnFingerDown(LeanFinger finger)
        {
            if (finger.IsOverGui)
                return;

            EventBus.Fire(new ScreenTouchedEvent());
            if (_isLocked || _dragged != null)
                return;

            var point = PointOnBoard(finger);
            var draggable = FindDraggable(point);
            if (draggable == null)
                return;

            _finger = finger;
            _dragged = draggable;
            _grabOffset = draggable.Position - point;
            draggable.BeginDrag();
        }

        private void OnFingerUpdate(LeanFinger finger)
        {
            if (finger == _finger)
                _dragged.DragTo(PointOnBoard(finger) + _grabOffset);
        }

        private void OnFingerUp(LeanFinger finger)
        {
            if (finger == _finger)
                Release();
        }

        private void Release()
        {
            _dragged.EndDrag();
            _dragged = null;
            _finger = null;
        }

        private IDraggable FindDraggable(Vector2 point)
        {
            IDraggable closest = null;
            var closestDistance = float.MaxValue;
            var count = Physics2D.OverlapCircleNonAlloc(point, _config.TouchRadius, _hits);
            for (var i = 0; i < count; i++)
            {
                var body = _hits[i].attachedRigidbody;
                if (body == null || !body.TryGetComponent(out IDraggable draggable))
                    continue;

                var distance = (_hits[i].ClosestPoint(point) - point).sqrMagnitude;
                if (distance >= closestDistance)
                    continue;

                closest = draggable;
                closestDistance = distance;
            }

            return closest;
        }

        private Vector2 PointOnBoard(LeanFinger finger)
        {
            var ray = finger.GetRay(_camera);
            _boardPlane.Raycast(ray, out var distance);
            return ray.GetPoint(distance);
        }
    }
}

using UnityEngine;

namespace Game.InputSystem
{
    public interface IDraggable
    {
        Vector2 Position { get; }
        void BeginDrag();
        void DragTo(Vector2 point);
        void EndDrag();
    }
}

using UnityEngine;

namespace Game.ExitSystem
{
    [CreateAssetMenu(menuName = "Game/Exit Config", fileName = "ExitConfig")]
    internal class ExitConfig : ScriptableObject
    {
        [SerializeField] private ExitView _exitPrefab;
        [SerializeField] private DoorView _doorPrefab;
        [SerializeField] private GameObject _arrowPrefab;

        public ExitView ExitPrefab => _exitPrefab;
        public DoorView DoorPrefab => _doorPrefab;
        public GameObject ArrowPrefab => _arrowPrefab;
    }
}

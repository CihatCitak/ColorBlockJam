using UnityEngine;

namespace Game.ExitSystem
{
    [CreateAssetMenu(menuName = "Game/Exit Config", fileName = "ExitConfig")]
    internal class ExitConfig : ScriptableObject
    {
        [SerializeField] private DoorView _doorPrefab;
        [SerializeField] private GameObject _arrowPrefab;

        public DoorView DoorPrefab => _doorPrefab;
        public GameObject ArrowPrefab => _arrowPrefab;
    }
}

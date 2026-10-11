using System.Collections.Generic;
using System.Linq;
using Game.BlockSystem;

namespace Game.LevelSystem
{
    internal static class LevelValidator
    {
        public static void Collect(LevelDraft draft, IReadOnlyList<DraftDoor> doors, List<string> warnings)
        {
            warnings.Clear();
            var blockColors = new HashSet<BlockColorData>(draft.Blocks.Select(block => block.Color));
            var doorColors = new HashSet<BlockColorData>(doors.Select(door => door.Color));

            foreach (var color in blockColors.Except(doorColors))
                warnings.Add($"{color.name} blocks have no door of their color.");

            foreach (var color in doorColors.Except(blockColors))
                warnings.Add($"{color.name} door has no block of its color.");

            foreach (var door in doors)
            {
                var along = LevelDraft.Along(door.Side);
                for (var i = 0; i < door.Length; i++)
                {
                    var cell = door.Cell + along * i;
                    var block = draft.BlockAt(cell);
                    if (block >= 0 && draft.Blocks[block].Color == door.Color)
                        warnings.Add($"{door.Color.name} block at {cell} starts in front of its own door.");
                }
            }
        }
    }
}

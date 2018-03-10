using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot.Primitives
{
    internal class Cell
    {
        public Location Location { get; set; }
        public CellType CellType { get; set; }
        public IView View { get; set; }

        public Cell(Location location, CellType cellType, IView view)
        {
            Location = location;
            CellType = cellType;
            View = view;
        }
    }
}

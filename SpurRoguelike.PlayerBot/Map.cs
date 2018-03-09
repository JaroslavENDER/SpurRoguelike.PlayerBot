using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using System.Collections.Generic;
using System.Linq;

namespace SpurRoguelike.PlayerBot
{
    internal class Map
    {
        private List<Cell> items;
        public int Count { get => items.Count; }
        public int LevelWidth { get; private set; }
        public int LevelHeight { get; private set; }

        public Map()
        {
            items = new List<Cell>();
        }

        public Cell Player { get => items.SingleOrDefault(cell => cell.View is PlayerView); }

        private void Add(Location location, CellType cellType, IView view)
        {
            var cell = GetCell(location);
            if (cell != null)
            {
                cell.CellType = cellType;
                cell.View = view;
            }
            else
            {
                items.Add(new Cell(location, cellType, view));
            }
        }

        public Cell GetCell(Location location)
        {
            return items.SingleOrDefault(c => c.Location == location);
        }
        public IEnumerable<Cell> GetCells()
        {
            return items as IEnumerable<Cell>;
        }

        public void Refresh(LevelView levelView)
        {
            LevelWidth = levelView.Field.Width;
            LevelHeight = levelView.Field.Height;
            if (Player != null)
                Player.View = null;

            for (var x = 0; x < levelView.Field.Width; x++)
                for (var y = 0; y < levelView.Field.Height; y++)
                {
                    var location = new Location(x, y);
                    var cellType = levelView.Field[location];
                    if (cellType == CellType.Hidden)
                        continue;
                    Add(location, cellType, null);
                }

            foreach (var view in levelView.HealthPacks)
            {
                var location = view.Location;
                var cellType = levelView.Field[location];
                Add(location, cellType, view);
            }
            foreach (var view in levelView.Items)
            {
                var location = view.Location;
                var cellType = levelView.Field[location];
                Add(location, cellType, view);
            }
            foreach (var view in levelView.Monsters)
            {
                var location = view.Location;
                var cellType = levelView.Field[location];
                Add(location, cellType, view);
            }
            var player = levelView.Player;
            var playerLocation = player.Location;
            var playerCellType = levelView.Field[playerLocation];
            Add(playerLocation, playerCellType, new PlayerView(player));
        }
    }
}

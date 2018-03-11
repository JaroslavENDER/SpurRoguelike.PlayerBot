using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using SpurRoguelike.PlayerBot.Primitives;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot.Infrastructure
{
    internal class Map
    {
        private Cell[,] items;
        private IMessageReporter logger;
        public int Level { get; private set; }
        public int LevelWidth { get; private set; }
        public int LevelHeight { get; private set; }

        public Map()
        {
            Level = 0;
        }
        public void SetLogger(IMessageReporter messageReporter)
        {
            logger = messageReporter;
        }

        public Cell Player { get; private set; }

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
                items[location.X, location.Y] = new Cell(location, cellType, view);
            }
        }

        public Cell GetCell(Location location)
        {
            if (location.X < 0 || location.X >= LevelWidth) return null;
            if (location.Y < 0 || location.Y >= LevelHeight) return null;
            return items[location.X, location.Y];
        }
        public IEnumerable<Cell> GetCells()
        {
            for (var x = 0; x < LevelWidth; x++)
                for (var y = 0; y < LevelHeight; y++)
                    yield return items[x, y];
        }

        public void Refresh(LevelView levelView)
        {
            if (LevelWidth != levelView.Field.Width || LevelHeight != levelView.Field.Height || PlayerIsTeleported(levelView.Player.Location)) //New level
            {
                LevelWidth = levelView.Field.Width;
                LevelHeight = levelView.Field.Height;
                items = new Cell[LevelWidth, LevelHeight];
                Level++;
                for (var x = 0; x < levelView.Field.Width; x++)
                    for (var y = 0; y < levelView.Field.Height; y++)
                    {
                        var location = new Location(x, y);
                        var cellType = levelView.Field[location];
                        Add(location, cellType, null);
                    }
            }
            else
            {
                for (var x = 0; x < levelView.Field.Width; x++)
                    for (var y = 0; y < levelView.Field.Height; y++)
                    {
                        var location = new Location(x, y);
                        var cellType = levelView.Field[location];
                        if (cellType == CellType.Hidden)
                            continue;
                        Add(location, cellType, null);
                    }
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
            Player = items[levelView.Player.Location.X, levelView.Player.Location.Y];
        }

        private bool PlayerIsTeleported(Location newLocation)
        {
            var offset = newLocation - Player.Location;
            if (offset.XOffset < -1 || offset.XOffset > 1) return true;
            if (offset.YOffset < -1 || offset.YOffset > 1) return true;
            return false;
        }
    }
}

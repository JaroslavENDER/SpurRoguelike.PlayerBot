using System;
using System.Collections.Generic;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using SpurRoguelike.PlayerBot.Primitives;

namespace SpurRoguelike.PlayerBot.Infrastructure
{
    internal class Navigator
    {
        private Map map;
        private IMessageReporter logger;
        private Offset[] offsetsToMove = new Offset[] 
        {
            new Offset(0, -1),
            new Offset(0, 1),
            new Offset(1, 0),
            new Offset(-1, 0),
        };
        private Offset[] offsetsToAttack = new Offset[] 
        {
            new Offset(-1, -1),
            new Offset(0, -1),
            new Offset(1, -1),
            new Offset(-1, 0),
            new Offset(1, 0),
            new Offset(-1, 1),
            new Offset(0, 1),
            new Offset(1, 1)
        };

        public Navigator(Map map)
        {
            this.map = map;
        }
        public void SetLogger(IMessageReporter messageReporter)
        {
            logger = messageReporter;
        }

        public Offset GetOffsetToAttack()
        {
            var path = GetPathTo(offsetsToAttack, cell => cell.View is PawnView);
            if (path.Count != 1) return default(Offset);
            return path.Pop();
        }

        public Stack<Offset> GetPathToTheMonster()
        {
            return GetPathTo(offsetsToMove, cell => cell.View is PawnView);
        }

        public Stack<Offset> GetPathToTheHealthPack()
        {
            return GetPathTo(offsetsToMove, cell => cell.View is HealthPackView, cell => cell.View is PawnView || cell.View is ItemView);
        }

        public Stack<Offset> GetPathToExit()
        {
            return GetPathTo(offsetsToMove, cell => cell.CellType == CellType.Exit, cell => cell.View is PawnView);
        }

        private Stack<Offset> GetPathTo(Offset[] offsets, Func<Cell, bool> searchPredicate, Func<Cell, bool> aviodPredicate = null)
        {
            var pathFromDirections = new Dictionary<Location, Offset>(); //Location and direction who brought here
            var pathFromLocations = new Dictionary<Location, Location>(); //Location and location from which he came here
            var currentLocation = default(Location);
            var currentCell = default(Cell);
            var searchSuccessful = false;
            var queue = new Queue<Location>();
            queue.Enqueue(map.Player.Location);

            while (queue.Count > 0)
            {
                currentLocation = queue.Dequeue();
                currentCell = map.GetCell(currentLocation);
                if (currentCell == null
                    || currentCell.CellType == CellType.Hidden || currentCell.CellType == CellType.Wall || currentCell.CellType == CellType.Trap
                    || (aviodPredicate != null && aviodPredicate.Invoke(currentCell)))
                    continue;
                if (searchPredicate(currentCell))
                {
                    searchSuccessful = true;
                    break;
                }
                foreach (var offset in offsets)
                {
                    var nextLocation = currentLocation + offset;
                    if (pathFromDirections.ContainsKey(nextLocation))
                        continue;
                    queue.Enqueue(nextLocation);
                    pathFromDirections.Add(nextLocation, offset);
                    pathFromLocations.Add(nextLocation, currentLocation);
                }
            }

            if (!searchSuccessful || pathFromDirections.Count == 0) return new Stack<Offset>();
            var result = new Stack<Offset>();
            do
            {
                result.Push(pathFromDirections[currentLocation]);
                currentLocation = pathFromLocations[currentLocation];
            } while (currentLocation != map.Player.Location);
            return result;
        }

        public bool CheckCellsAround(Location startLocation, Func<Cell, bool> searchPredicate)
        {
            foreach (var offset in offsetsToAttack)
            {
                var location = startLocation + offset;
                var cell = map.GetCell(location);
                if (searchPredicate(cell))
                    return true;
            }
            return false;
        }

        public bool IsHaveMonsters()
        {
            foreach (var cell in map.GetCells())
                if (cell.View is PawnView)
                    return true;
            return false;
        }

        public bool IsHaveHealthPacks()
        {
            foreach (var cell in map.GetCells())
                if (cell.View is HealthPackView)
                    return true;
            return false;
        }
    }
}

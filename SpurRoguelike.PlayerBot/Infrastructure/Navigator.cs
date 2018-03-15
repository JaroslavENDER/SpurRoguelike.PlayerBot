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
        private Offset[] offsetsToMoveToLevel5 = new Offset[]
        {
            new Offset(0, 1),
            new Offset(0, -1),
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
        private Random rand = new Random();

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
            logger.ReportMessage("GetOffsetToAttack()");
            return GetPathTo(offsetsToAttack, cell => cell.View is PawnView).Pop();
        }

        public Stack<Offset> GetPathToTheMonster()
        {
            logger.ReportMessage("GetPathToTheMonster()");
            return GetPathTo(offsetsToMove, cell => cell.View is PawnView);
        }

        public Stack<Offset> GetPathToTheHealthPack()
        {
            logger.ReportMessage("GetPathToTheHealthPack()");
            return GetPathTo(offsetsToMove, cell => cell.View is HealthPackView, cell => cell.View is PawnView || cell.View is ItemView);
        }

        public Stack<Offset> GetPathToExit()
        {
            logger.ReportMessage("GetPathToExit()");
            return GetPathTo(offsetsToMove, cell => cell.CellType == CellType.Exit, cell => cell.View is PawnView);
        }

        public Stack<Offset> GetPathToTheObscurity()
        {
            logger.ReportMessage("GetPathToTheObscurity()");
            return GetPathTo(offsetsToMove, cell => cell.CellType == CellType.Hidden, cell => cell.View is PawnView);
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
            if (map.Level == 5) offsets = offsetsToMoveToLevel5;

            while (queue.Count > 0)
            {
                currentLocation = queue.Dequeue();
                currentCell = map.GetCell(currentLocation);
                if (currentCell != map.Player)
                    if (currentCell == null
                        || currentCell.CellType == CellType.Wall || currentCell.CellType == CellType.Trap
                        || (aviodPredicate != null && aviodPredicate.Invoke(currentCell)))
                        continue;
                if (searchPredicate(currentCell))
                {
                    searchSuccessful = true;
                    break;
                }
                //Mix(offsets);
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

        private void Mix(Offset[] offsets)
        {
            for (var i = 0; i < offsets.Length; i++)
            {
                var a = rand.Next(offsets.Length);
                var b = rand.Next(offsets.Length);
                var temp = offsets[a];
                offsets[a] = offsets[b];
                offsets[b] = temp;
            }
        }

        public bool CheckCellsAround(Location startLocation, Func<Cell, bool> searchPredicate)
        {
            foreach (var offset in offsetsToAttack)
            {
                var cell = map.GetCell(startLocation + offset);
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

using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot.Infrastructure
{
    internal class Autopilot
    {
        private Map map;
        private Stack<Offset> path;
        private IMessageReporter logger;
        public bool IsActive { get; private set; }

        public Autopilot(Map map)
        {
            this.map = map;
        }
        public void SetLogger(IMessageReporter messageReporter)
        {
            logger = messageReporter;
        }

        public void Activate(Stack<Offset> path)
        {
            IsActive = true;
            this.path = path;
        }

        private Turn Stop(Offset? lastOffset = null)
        {
            IsActive = false;
            if (lastOffset.HasValue)
                return Turn.Step(lastOffset.Value);
            return null;
        }

        public Turn GetTurn()
        {
            if (path.Count == 0)
                return Stop();
            var offset = path.Pop();
            var nextCell = map.GetCell(map.Player.Location + offset);
            if (nextCell.CellType == CellType.Trap ||
                nextCell.CellType == CellType.Wall ||
                nextCell.View is PawnView)
                return Stop();
            if (path.Count < 10)
                return Stop(offset);
            return Turn.Step(offset);
        }
    }
}

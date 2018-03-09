using System.Threading;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot
{
    public class PlayerBot : IPlayerController
    {
        private Map map;

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            if (map == null) map = new Map();
            Thread.Sleep(500);

            map.Refresh(levelView);



            return Turn.Step(StepDirection.North);
        }
    }
}

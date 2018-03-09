using System.Threading;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot
{
    public class PlayerBot : IPlayerController
    {
        private Map map;
        private Navigator navigator;

        public PlayerBot()
        {
            map = new Map();
            navigator = new Navigator(map);
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            //Thread.Sleep(100);

            navigator.Refresh(levelView, messageReporter);

            if (levelView.Player.Health < 50)
                return navigator.GoToTheHealthPack();
            if (navigator.CheckCellsAround(map.PlayerLocation, typeof(PawnView)))
                return navigator.Attack();
            return navigator.GoToTheMonster();
        }
    }
}

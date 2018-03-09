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
            navigator.Refresh(levelView, messageReporter);

            if (levelView.Player.Health < 70)
            {
                if (navigator.IsHaveHealthPacks())
                    return navigator.GoToTheHealthPack();
                return navigator.GoToExit();
            }
            if (navigator.IsHaveMonsters())
            {
                if (navigator.CheckCellsAround(map.Player.Location, typeof(PawnView)))
                    return navigator.Attack();
                return navigator.GoToTheMonster();
            }

            if (navigator.IsHaveHealthPacks())
                return navigator.GoToTheHealthPack();
            return navigator.GoToExit();
        }
    }
}

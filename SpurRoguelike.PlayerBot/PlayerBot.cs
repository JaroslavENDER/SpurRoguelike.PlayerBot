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
                    return Turn.Step(navigator.GetPathToTheHealthPack().Pop());
                return Turn.Step(navigator.GetPathToExit().Pop());
            }
            if (navigator.IsHaveMonsters())
            {
                if (navigator.CheckCellsAround(map.Player.Location, cell => cell?.View is PawnView))
                    return Turn.Attack(navigator.GetOffsetToAttack());
                return Turn.Step(navigator.GetPathToTheMonster().Pop());
            }

            if (navigator.IsHaveHealthPacks())
                return Turn.Step(navigator.GetPathToTheHealthPack().Pop());
            return Turn.Step(navigator.GetPathToExit().Pop());
        }
    }
}

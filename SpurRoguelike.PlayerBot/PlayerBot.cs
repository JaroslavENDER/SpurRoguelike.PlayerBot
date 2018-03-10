using System;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot
{
    public class PlayerBot : IPlayerController
    {
        private Map map;
        private Navigator navigator;
        private Autopilot autopilot;

        public PlayerBot()
        {
            map = new Map();
            navigator = new Navigator(map);
            autopilot = new Autopilot(map);
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            map.Refresh(levelView);
            map.SetLogger(messageReporter);
            navigator.SetLogger(messageReporter);
            autopilot.SetLogger(messageReporter);

            //System.Threading.Thread.Sleep(500);

            if (autopilot.IsActive)
            {
                var turn = autopilot.GetTurn();
                if (turn != null)
                    return turn;
            }

            if (levelView.Player.Health < 70)
                return GoToTheHealth();
            if (navigator.IsHaveMonsters())
                return GoToTheMonster();

            return GoToTheEnd();
        }

        private Turn GoToTheHealth()
        {
            if (navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack(), isSafePath: true);
            else
                autopilot.Activate(navigator.GetPathToExit(), isSafePath: true);
            return autopilot.GetTurn();
        }

        private Turn GoToTheMonster()
        {
            if (navigator.CheckCellsAround(map.Player.Location, cell => cell?.View is PawnView))
                return Turn.Attack(navigator.GetOffsetToAttack());
            autopilot.Activate(navigator.GetPathToTheMonster(), isSafePath: true);
            return autopilot.GetTurn();
        }

        private Turn GoToTheEnd()
        {
            if (navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack(), isSafePath: true);
            else
                autopilot.Activate(navigator.GetPathToExit(), isSafePath: true);
            return autopilot.GetTurn();
        }
    }
}

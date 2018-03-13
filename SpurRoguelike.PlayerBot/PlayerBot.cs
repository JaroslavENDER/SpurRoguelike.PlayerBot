using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using SpurRoguelike.PlayerBot.Infrastructure;

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

            if (map.Level == 5 && levelView.Player.Health < 10) System.Threading.Thread.Sleep(500);

            if (autopilot.IsActive)
            {
                var turn = autopilot.GetTurn();
                if (turn != null)
                    return turn;
            }

            if (levelView.Player.Health < 70)
                return GoToTheHealth(levelView, messageReporter);
            if (navigator.IsHaveMonsters())
                return GoToTheMonster(levelView, messageReporter);

            return GoToTheEnd(levelView, messageReporter);
        }

        private Turn GoToTheHealth(LevelView levelView, IMessageReporter messageReporter)
        {
            if (navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack(levelView.Player.Health));
            else
                autopilot.Activate(navigator.GetPathToExit());
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheMonster(LevelView levelView, IMessageReporter messageReporter)
        {
            if (navigator.CheckCellsAround(map.Player.Location, cell => cell?.View is PawnView))
                return Turn.Attack(navigator.GetOffsetToAttack());
            autopilot.Activate(navigator.GetPathToTheMonster());
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheEnd(LevelView levelView, IMessageReporter messageReporter)
        {
            if (levelView.Player.Health < 100 && navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack());
            else
                autopilot.Activate(navigator.GetPathToExit());
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheObscurity(LevelView levelView, IMessageReporter messageReporter)
        {
            messageReporter.ReportMessage(" Infinity is not limit!!!");

            var path = navigator.GetPathToTheObscurity();
            if (path.Count != 0)
            {
                autopilot.Activate(path);
                return autopilot.GetTurn();
            }
            var offset = navigator.GetOffsetToAttack();
            if (offset != default(Offset))
                return Turn.Attack(offset);
            path = navigator.GetPathToTheMonster();
            autopilot.Activate(path);
            return autopilot.GetTurn() ?? Turn.None;
        }
    }
}

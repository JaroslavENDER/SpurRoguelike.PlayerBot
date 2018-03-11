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
        private IMessageReporter logger;

        public PlayerBot()
        {
            map = new Map();
            navigator = new Navigator(map);
            autopilot = new Autopilot(map);
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            logger = messageReporter;
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
                return GoToTheHealth(levelView, messageReporter);
            if (navigator.IsHaveMonsters())
                return GoToTheMonster(levelView, messageReporter);

            return GoToTheEnd(levelView, messageReporter);
        }

        private Turn GoToTheHealth(LevelView levelView, IMessageReporter messageReporter)
        {
            if (navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack(), isSafePath: true);
            else
                autopilot.Activate(navigator.GetPathToExit(), isSafePath: true);
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheMonster(LevelView levelView, IMessageReporter messageReporter)
        {
            if (navigator.CheckCellsAround(map.Player.Location, cell => cell?.View is PawnView))
                return Turn.Attack(navigator.GetOffsetToAttack());
            autopilot.Activate(navigator.GetPathToTheMonster(), isSafePath: true);
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheEnd(LevelView levelView, IMessageReporter messageReporter)
        {
            if (levelView.Player.Health < 100 && navigator.IsHaveHealthPacks())
                autopilot.Activate(navigator.GetPathToTheHealthPack(), isSafePath: true);
            else
                autopilot.Activate(navigator.GetPathToExit(), isSafePath: true);
            return autopilot.GetTurn() ?? GoToTheObscurity(levelView, messageReporter);
        }

        private Turn GoToTheObscurity(LevelView levelView, IMessageReporter messageReporter)
        {
            var path = navigator.GetPathToTheObscurity();
            logger.ReportMessage(path.Count.ToString());
            autopilot.Activate(path, isSafePath: true);
            return autopilot.GetTurn() ?? Turn.None;
        }
    }
}

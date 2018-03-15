using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using System.Reflection;

namespace SpurRoguelike.UnfairBot
{
    public class UnfairBot : IPlayerController
    {
        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            var level = levelView.GetType().GetField("level", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(levelView) as Level;
            level.Complete();
            return Turn.None;
        }
    }
}
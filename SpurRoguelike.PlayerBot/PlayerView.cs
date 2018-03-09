using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot
{
    internal class PlayerView : IView
    {
        private PawnView player;

        public PlayerView(PawnView player)
        {
            this.player = player;
        }

        public bool HasValue => player.HasValue;
    }
}

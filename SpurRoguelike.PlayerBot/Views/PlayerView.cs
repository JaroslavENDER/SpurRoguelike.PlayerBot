using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace SpurRoguelike.PlayerBot.Views
{
    internal struct PlayerView : IView
    {
        private PawnView player;

        public PlayerView(PawnView player)
        {
            this.player = player;
        }

        public Location Location => player.Location;

        public bool HasValue => player.HasValue;
    }
}

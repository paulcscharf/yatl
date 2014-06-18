using amulware.Graphics;
using OpenTK;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    class Crystal : Unit
    {
        private readonly float range;
        private readonly Color color;

        public Crystal(GameState game, Vector2 position)
            : base(game, position, 0)
        {
            var tileLight = this.Tile.Info.Lightness;

            this.range = (0.5f + tileLight) * GlobalRandom.NextFloat(8, 9);

            this.color = Color.FromHSVA(GlobalRandom.NextFloat(0.6f, 0.8f) * GameMath.TwoPi, 0.1f, 1f);
        }

        public override void Draw(SpriteManager sprites)
        {
            sprites.PointLight.Draw(this.position.WithZ(2f), this.color, 1, this.range);
        }
    }
}

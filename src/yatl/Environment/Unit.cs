using System;
using OpenTK;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using amulware.Graphics;
using Hex = yatl.Settings.Game.Level;

namespace yatl.Environment
{

    abstract class Unit : GameObject, IPositionable
    {
        protected Vector2 position;
        public Vector2 Position { get { return this.position; } }
        protected Vector2 velocity;
        public Vector2 Velocity { get { return this.velocity; } }

        private readonly float frictionCoefficient;

        private Tile<TileInfo> tile;
        public Tile<TileInfo> Tile { get { return this.tile; } }

        private Vector2 tileCenter;

        public Unit(GameState game, Vector2 position, float frictionCoefficient = 10)
            : base(game)
        {
            this.frictionCoefficient = frictionCoefficient;
            this.position = position;

            this.setTile(game.Level.GetTile(position));
        }

        private void setTile(Tile<TileInfo> tile)
        {
            this.tile = tile;
            this.tileCenter = this.game.Level.GetPosition(tile);
        }

        public override void Update(GameUpdateEventArgs e)
        {
            // don't move stationary objects
            if (this.velocity == Vector2.Zero)
                return;

            // update position and velocity
            this.position += this.velocity * e.ElapsedTimeF;

            float slowDownFactor = 1 - this.frictionCoefficient * e.ElapsedTimeF;
            this.velocity *= slowDownFactor < 0 ? 0 : slowDownFactor;

            // update tile
            this.updateTile();
        }

        private void updateTile()
        {
            while (true)
            {
                var fromTileCenter = this.position - this.tileCenter;

                var fromTileCenterAbs = new Vector2(Math.Abs(fromTileCenter.X), Math.Abs(fromTileCenter.Y));

                if (fromTileCenterAbs.X <= Hex.HexagonWidth * 0.5f
                    && Hex.HexagonSide - fromTileCenterAbs.X
                    * (Hex.HexagonSide / Hex.HexagonWidth)
                    >= fromTileCenterAbs.Y)
                    return;

                this.setTile(this.tile.Neighbour(
                    Utilities.Direction.Of(fromTileCenter).Hexagonal()
                    ));
            }
        }

        public override void Draw(SpriteManager sprites)
        {
            sprites.Bloob.Color = Color.DeepPink;
            sprites.Bloob.DrawSprite(this.position, 0, 1);
        }
    }
}

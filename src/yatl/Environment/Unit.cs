using System;
using OpenTK;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using amulware.Graphics;

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
                var dSquared = fromTileCenter.LengthSquared;

                if (dSquared < Settings.Game.Level.HexagonInnerRadiusSquared)
                    return;

                var fromTileCenterAbs = new Vector2(Math.Abs(fromTileCenter.X), Math.Abs(fromTileCenter.Y));

                if (fromTileCenterAbs.X < Settings.Game.Level.HexagonWidth * 0.5f
                    && Settings.Game.Level.HexagonSide - fromTileCenterAbs.X
                    * (Settings.Game.Level.HexagonSide / Settings.Game.Level.HexagonWidth)
                    > fromTileCenterAbs.Y)
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

    abstract class GameObject
    {
        protected readonly GameState game;

        public GameObject(GameState game)
        {
            this.game = game;
            game.AddObject(this);
        }

        public abstract void Update(GameUpdateEventArgs e);
        public abstract void Draw(SpriteManager sprites);

        public void Delete()
        {
            this.Deleted = true;
        }

        public virtual void Dispose()
        {

        }

        public bool Deleted { get; private set; }
    }
}

using System;
using OpenTK;
using yatl.Environment.Level;
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
        public Vector2 TileCenter { get { return this.tileCenter; } }

        public Unit(GameState game, Vector2 position, float frictionCoefficient = 10)
            : base(game)
        {
            this.frictionCoefficient = frictionCoefficient;
            this.position = position;

            this.setTile(game.Level.GetTile(position));
        }

        protected virtual void setTile(Tile<TileInfo> tile)
        {
            if (!tile.IsValid)
            {
                throw new ArgumentException("Tile not valid.", "tile");
            }
                

            this.tile = tile;
            this.tileCenter = this.game.Level.GetPosition(tile);
        }

        public override void Update(GameUpdateEventArgs e)
        {
            // don't move stationary objects
            if (this.velocity == Vector2.Zero)
                return;

            var step = this.velocity * e.ElapsedTimeF;

            Vector2 lastStepDir = Vector2.Zero;

            // update position (tests collision recursively)
            for (int i = 0; i < 5; i++)
            {
                var rayResult = new RayHitResult(false, 1, this.position - this.tileCenter + step, Vector2.Zero);
                while(true)
                {
                    var result = this.tile.Info.ShootRay(new Ray(this.position - this.tileCenter, step));

                    if (result.RayFactor < rayResult.RayFactor)
                        rayResult = result;

                    var point = rayResult.Point + this.tileCenter;

                    var switchedTile = this.updateTile(rayResult.Point);

                    if (!switchedTile)
                        break;

                    rayResult = rayResult.WithNewPoint(point - this.tileCenter);
                }

                lastStepDir = this.tileCenter + rayResult.Point - this.position;

                this.position = this.tileCenter + rayResult.Point;

                if (!rayResult.Hit)
                    break;


                this.position += rayResult.Normal * 0.01f;

                var projected = rayResult.Normal * Vector2.Dot(rayResult.Normal, step);

                step -= projected;
            }

            this.velocity = lastStepDir.LengthSquared > 0 ?
                this.velocity.Length * lastStepDir.Normalized() : Vector2.Zero;

            float slowDownFactor = 1 - this.frictionCoefficient * e.ElapsedTimeF;
            this.velocity *= slowDownFactor < 0 ? 0 : slowDownFactor;
        }

        private bool updateTile(Vector2 newPositionRelative)
        {
            var fromTileCenterAbs = new Vector2(Math.Abs(newPositionRelative.X), Math.Abs(newPositionRelative.Y));

            if (fromTileCenterAbs.X <= Hex.HexagonWidth * 0.5f
                && Hex.HexagonSide - fromTileCenterAbs.X
                * (Hex.HexagonSide / Hex.HexagonWidth)
                >= fromTileCenterAbs.Y)
                return false;

            var newTile = this.tile.Neighbour(
                Utilities.Direction.Of(newPositionRelative).Hexagonal());

            this.setTile(newTile);

            return true;
        }

        public override void Draw(SpriteManager sprites)
        {
            sprites.Bloob.Color = Color.DeepPink;
            sprites.Bloob.DrawSprite(this.position, 0, 2);
        }
    }
}

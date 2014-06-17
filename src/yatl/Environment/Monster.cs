using System;
using System.Linq;
using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level;
using yatl.Environment.Tilemap.Hexagon;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class Monster : Unit
    {
        private bool chasing;
        private float nextVisibleCheck;
        private bool seesPlayer;
        private Vector2 lastKnownPlayerPosition;
        private float losePlayerTime;

        private float nextHitTime;

        public Monster(GameState game, Vector2 position)
            : base(game, position, Settings.Game.Enemy.FrictionCoefficient)
        {

        }

        public override void Update(GameUpdateEventArgs e)
        {
            var toPlayer = this.game.Player.Position - this.position;


            if (this.nextVisibleCheck == 0)
                this.nextVisibleCheck = this.game.Time +
                    GlobalRandom.NextFloat(Settings.Game.Enemy.MinScanInterval, Settings.Game.Enemy.MaxScanInterval);
            if (this.nextVisibleCheck < this.game.Time)
            {
                this.nextVisibleCheck = 0;
                if (toPlayer.LengthSquared < Settings.Game.Enemy.ViewDistanceSquared)
                {
                    var result = this.game.Level.ShootRay(new Ray(this.position, toPlayer), this.Tile);
                    this.seesPlayer = !result.Results.Hit;

                    if (game.DrawDebug)
                    {
                        var lines = SpriteManager.Instance.Lines;
                        lines.Color = result.Results.Hit ? Color.Red : Color.Green;
                        lines.LineWidth = 0.2f;
                        lines.DrawLine(this.position, result.GlobalPoint);
                    }
                }
            }

            if (this.seesPlayer)
            {
                if (!this.chasing)
                {
                    this.game.ChasingEnemies.Add(this);
                    this.chasing = true;
                }
                this.lastKnownPlayerPosition = this.game.Player.Position;
                this.losePlayerTime = this.game.Time + 1;
            }

            if (this.chasing && this.losePlayerTime < this.game.Time)
            {
                this.game.ChasingEnemies.Remove(this);
                this.chasing = false;
            }

            if (this.chasing)
            {
                var toKnownPlayerPosition = this.lastKnownPlayerPosition - this.position;
                this.velocity += toKnownPlayerPosition.Normalized()
                    * Settings.Game.Enemy.Acceleration * e.ElapsedTimeF;

            }

            foreach (var monster in this.Tile.Info.Monsters)
            {
                if(monster == this)
                    continue;
                var diff = monster.position - this.position;
                var dSquared = diff.LengthSquared;
                if(dSquared == 0)
                    continue;
                const float radius = 1f;
                if (dSquared < radius * radius)
                {
                    var d = (float)Math.Sqrt(dSquared);
                    var normalDiff = diff / d;
                    var f = radius - d;
                    f *= f;
                    this.velocity -= 100 * normalDiff * f * e.ElapsedTimeF;

                    if (this.game.DrawDebug)
                    {
                        var lines = SpriteManager.Instance.Lines;
                        lines.Color = Color.Red;
                        lines.LineWidth = 0.1f;
                        lines.DrawLine(this.position, this.position + diff / d);   
                    }
                }
            }

            foreach (var tile in this.Tile.Info.OpenSides.Enumerate()
                .Select(d => this.Tile.Neighbour(d)).Append(this.Tile))
            {
                var info = tile.Info;

                if (info.Lightness > 0.2)
                {
                    var tilePosition = this.game.Level.GetPosition(tile);

                    var diff = tilePosition - this.position;
                    var d = diff.Length;
                    const float radius = Settings.Game.Level.HexagonSide * 1.1f;
                    if (d < radius)
                    {
                        var normalDiff = diff / d;
                        var f = radius - d;
                        f *= f;
                        this.velocity -= 100 * normalDiff * f * e.ElapsedTimeF;
                    }
                }
            }

            if (this.nextHitTime <= this.game.Time && toPlayer.LengthSquared < Settings.Game.Enemy.HitDistanceSquared)
            {
                this.game.Player.Damage(Settings.Game.Enemy.HitDamage);
                this.nextHitTime = this.game.Time + Settings.Game.Enemy.HitInterval;
            }

            base.Update(e);
        }

        protected override void setTile(Tile<TileInfo> tile)
        {
            if(this.Tile.IsValid)
                this.Tile.Info.Monsters.Remove(this);
            base.setTile(tile);

            this.Tile.Info.Monsters.Add(this);
        }

        public override void Draw(SpriteManager sprites)
        {
            base.Draw(sprites);


            if (this.game.DrawDebug && this.chasing)
            {
                var argb = this.seesPlayer ? Color.Silver : Color.Gray;
                sprites.EmptyHexagon.Color = argb;
                sprites.EmptyHexagon.DrawSprite(this.lastKnownPlayerPosition, 0, 1);

                sprites.Lines.Color = argb;
                sprites.Lines.LineWidth = 0.1f;
                sprites.Lines.DrawLine(this.position, this.lastKnownPlayerPosition);
            }
        }
    }
}

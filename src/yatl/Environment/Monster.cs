using amulware.Graphics;
using OpenTK;
using yatl.Environment.Level;
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
                this.chasing = true;
                this.lastKnownPlayerPosition = this.game.Player.Position;
                this.losePlayerTime = this.game.Time + 1;
            }

            if (this.losePlayerTime < this.game.Time)
                this.chasing = false;

            if (this.chasing)
            {
                var toKnownPlayerPosition = this.lastKnownPlayerPosition - this.position;
                this.velocity += toKnownPlayerPosition.Normalized()
                    * Settings.Game.Enemy.Acceleration * e.ElapsedTimeF;

            }

            base.Update(e);
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

using System;
using amulware.Graphics;
using OpenTK;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class Wisp : Unit
    {
        private readonly ControlScheme controls;

        private float health;
        private float healStartTime;

        public Wisp(GameState game, Vector2 position)
            : base(game, position, Settings.Game.Wisp.FrictionCoefficient)
        {
            this.controls = new ControlScheme();
            this.health = Settings.Game.Wisp.MaxHealth * 0.1f;
            this.healStartTime = -1000;
        }

        public override void Update(GameUpdateEventArgs e)
        {
            var acceleration = new Vector2(
                this.controls.Right.AnalogAmount - this.controls.Left.AnalogAmount,
                this.controls.Up.AnalogAmount - this.controls.Down.AnalogAmount
                );
            var a = acceleration.Length;
            if (a > 0)
                acceleration /= a;

            this.velocity += acceleration * Settings.Game.Wisp.Acceleration * e.ElapsedTimeF;

            base.Update(e);

            if (this.healStartTime <= this.game.Time)
            {
                this.health = Math.Min(Settings.Game.Wisp.MaxHealth, this.health + e.ElapsedTimeF * Settings.Game.Wisp.HealSpeed);
            }

            if (this.game.State != GameState.GameOverState.Undetermined)
                return;

            if (this.health <= 0)
            {
                this.game.GameOver(false);
            }
            else if (this.Tile.Radius == this.game.Level.Tilemap.Radius)
            {
                this.game.GameOver(true);
            }
        }

        public void Damage(float damage)
        {
            this.health -= damage;
            this.healStartTime = this.game.Time + 1;
        }

        public override void Draw(SpriteManager sprites)
        {
            if (this.game.DrawDebug)
            {
                var v = this.game.Level.GetPosition(this.Tile);

                var geo = sprites.EmptyHexagon;
                geo.Color = new Color(Color.Green, 0);
                geo.DrawSprite(v, 0, Settings.Game.Level.HexagonDiameter);
            }

            var healthPercentage = Math.Max(0, this.health / Settings.Game.Wisp.MaxHealth);

            var light = GlobalRandom.NextFloat(healthPercentage * healthPercentage, healthPercentage);

            sprites.PointLight.Draw(this.position.WithZ(1.5f), Color.LightYellow, 1f, 15 * light);

            base.Draw(sprites);
        }
    }
}

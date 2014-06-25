using System;
using System.Security.Cryptography;
using amulware.Graphics;
using Newtonsoft.Json.Serialization;
using OpenTK;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class Particle : GameObject
    {
        private readonly IPositionable parent;
        private readonly Color color;
        private readonly float size;

        private Vector3 position;
        private Vector3 velocity;

        private bool relativePosition = true;

        private float fadeOutFactor;
        private float fadeOutStart;

        private float alpha;

        public Particle(GameState game, IPositionable parent,
            Vector3 position, Vector3 velocity, Color color, float size)
            : base(game)
        {
            this.parent = parent;
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.size = size;
        }

        private Vector3 parentPosition
        {
            get { return this.parent.Position.WithZ(1.5f); }
        }

        public void DetachFromParent()
        {
            if (!this.relativePosition)
                return;

            this.position += this.parentPosition;

            this.relativePosition = false;
        }

        public void FadeAway(float delay)
        {
            this.fadeOutStart = this.game.Time + delay;
            this.fadeOutFactor = GlobalRandom.NextFloat(1, 3);
        }

        public void Push(Vector3 impulse)
        {
            this.velocity += impulse;
        }

        public override void Update(GameUpdateEventArgs e)
        {
            var toParent = this.relativePosition
                ? -this.position
                : (this.parentPosition - this.position);

            var dSquared = toParent.LengthSquared;

            var distance = (float)Math.Sqrt(dSquared);

            var force = 10;

            this.velocity += toParent * (1 / distance * force * e.ElapsedTimeF);


            var tangent = Vector3.Cross(toParent, Vector3.UnitZ);

            var tangentForce = 0.3f * Math.Min(1, 1 / dSquared);

            if (Vector3.Dot(tangent, this.velocity) > 0)
                tangentForce *= -1;

            this.velocity += tangent.Normalized() * tangentForce * e.ElapsedTimeF;


            var speedSquared = this.velocity.LengthSquared;

            if (speedSquared > 10)
                this.velocity *= (float)Math.Pow(0.5f, e.ElapsedTimeF);


            this.position += this.velocity * e.ElapsedTimeF;

            if (this.fadeOutStart == 0 || this.fadeOutStart > this.game.Time)
            {
                this.alpha += (1 - this.alpha) * e.ElapsedTimeF;
            }
            else
            {
                this.alpha *= (1 - this.fadeOutFactor * e.ElapsedTimeF);
                if(this.alpha < 0.01f)
                    this.Delete();
            }
        }

        public override void Draw(SpriteManager sprites)
        {
            var blink = sprites.Blink;

            var p = this.relativePosition
                ? this.parentPosition + this.position
                : this.position;

            blink.Color = this.color * this.alpha;
            blink.DrawSprite(p, 0, this.size);
        }

    }
}

using System.Collections.Generic;
using amulware.Graphics;
using OpenTK;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment
{
    sealed class ParticleCloud
    {
        private readonly IPositionable parent;
        private readonly GameState game;
        private readonly Color color;
        private readonly float turnOverRate;
        private readonly float size;

        private List<Particle> particles = new List<Particle>();


        public ParticleCloud(GameState game, int averageCount, IPositionable parent, Color color, float turnOverRate, float size)
        {
            this.parent = parent;
            this.game = game;
            this.color = color;
            this.turnOverRate = turnOverRate;
            this.size = size;

            for (int i = 0; i < averageCount; i++)
            {
                this.particles.Add(this.makeParticle());
            }
        }

        private Particle makeParticle()
        {
            return new Particle(this.game, this.parent,
                GameMath.Vector3FromRotation(GlobalRandom.Angle(), GlobalRandom.Angle(), this.size)
                * new Vector3(1, 1, 0.5f),
                new Vector3(),
                color, GlobalRandom.NextFloat(0.8f, 1.2f) * this.size);
        }

        public void Update(GameUpdateEventArgs e)
        {
            var detachCount = randomCount(this.turnOverRate * e.ElapsedTimeF);

            for (int j = 0; j < detachCount; j++)
            {
                var i = GlobalRandom.Next(particles.Count);

                this.particles[i].DetachFromParent();
                this.particles[i].FadeAway(GlobalRandom.NextFloat(0.2f, 1f));

                this.particles[i] = this.makeParticle();
            }

        }

        public void Explode(float percentage, Vector3 impulse, float randomImpulse)
        {
            var detachCount = randomCount(this.particles.Count * percentage);

            for (int j = 0; j < detachCount; j++)
            {
                var i = GlobalRandom.Next(particles.Count);

                var oldParticle = this.particles[i];

                oldParticle.DetachFromParent();
                oldParticle.FadeAway(GlobalRandom.NextFloat(0.2f, 0.5f));
                oldParticle.Push(impulse +
                    GameMath.Vector2FromRotation(GlobalRandom.Angle(),
                    GlobalRandom.NextFloat(randomImpulse)).WithZ(0));

                this.particles[i] = this.makeParticle();
            }
        }

        private static int randomCount(float randomCountFloat)
        {
            var randomCount = (int)randomCountFloat;

            if (GlobalRandom.NextBool(randomCountFloat - randomCount))
                randomCount++;

            return randomCount;
        }

        public void Draw(SpriteManager sprites)
        {
            
        }
    }
}

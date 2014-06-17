using System;
using amulware.Graphics;
using OpenTK;
using yatl.Input;
using yatl.Rendering;
using yatl.Utilities;

namespace yatl.Environment.Hud
{
    class Scrollbar
    {
        private readonly Action<float> updateValue;
        private readonly Vector3 position;
        private readonly IAction decrease;
        private readonly IAction increase;

        private float value;

        private float drawValue;

        private float nextAutoLeft;
        private float nextAutoRight;

        private bool isMovingLeft;
        private bool isMovingRight;

        public Scrollbar(Vector3 position, float value, Action<float> updateValue,
            IAction decrease, IAction increase)
        {
            this.position = position;
            this.value = value;
            this.drawValue = value;
            this.updateValue = updateValue;
            this.decrease = decrease;
            this.increase = increase;
        }

        public float Value
        {
            get { return this.value; }
            set { this.value = GameMath.Clamp(value, 0, 1); }
        }

        public void Update(GameUpdateEventArgs e)
        {
            float oldValue = this.value;

            const float firstAutoStepDelay = 0.1f;
            const float autoStepDelay = 0.04f;
            const float stepSize = 0.05f;
            const float autoStepSize = 0.05f;

            #region decrease
            if (this.decrease.Hit)
            {
                this.nextAutoLeft = firstAutoStepDelay;
            }

            if (this.decrease.Active)
            {
                this.nextAutoLeft -= e.ElapsedTimeF;

                if (this.nextAutoLeft <= 0)
                {
                    this.isMovingLeft = true;
                    this.nextAutoLeft = autoStepDelay;
                    this.value -= autoStepSize;
                }
            }

            if (this.decrease.Released)
            {
                if (!this.isMovingLeft)
                    this.value -= stepSize;
                this.isMovingLeft = false;
            }
            #endregion

            #region increase
            if (this.increase.Hit)
            {
                this.nextAutoRight = firstAutoStepDelay;
            }

            if (this.increase.Active)
            {
                this.nextAutoRight -= e.ElapsedTimeF;

                if (this.nextAutoRight <= 0)
                {
                    this.isMovingRight = true;
                    this.nextAutoRight = autoStepDelay;
                    this.value += autoStepSize;
                }
            }

            if (this.increase.Released)
            {
                if (!this.isMovingRight)
                    this.value += stepSize;
                this.isMovingRight = false;
            }
            #endregion

            if (this.value != oldValue)
            {
                this.value = (float)Math.Round(GameMath.Clamp(this.value, 0, 1), 2);
            }

            if (this.value != oldValue)
            {
                this.updateValue(this.value);
            }

            this.drawValue += (this.value - this.drawValue) * 15f * e.ElapsedTimeF;
        }

        public void Draw(SpriteManager sprites)
        {
            const float height = 0.5f;
            const float width = 10f;

            sprites.Hud.Color = Color.Gray;
            sprites.Hud.DrawBarH(this.position, width, height);

            if (this.drawValue > 0.02f)
            {
                sprites.Hud.DrawFilledBarH(this.position, width * this.drawValue, height);
            }
        }
    }
}

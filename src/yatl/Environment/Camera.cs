using System;
using OpenTK;

namespace yatl.Environment
{
    sealed class Camera
    {
        private readonly IPositionable focus;

        public Vector3 Focus { get; private set; }
        public Vector3 Position { get; private set; }

        public bool Zoom { get; set; }

        public Camera(IPositionable focus)
        {
            this.focus = focus;
        }

        public void Update(GameUpdateEventArgs e)
        {
            var focusGoal = new Vector3(this.focus.Position) + Settings.Game.Camera.FocusOffset;
            var positionGoal = new Vector3(this.focus.Position) + Settings.Game.Camera.PositionOffset
                * (this.Zoom ? Settings.Game.Camera.OverviewZoom : Settings.Game.Camera.DefaultZoom);

            var focusSpeed = Math.Min(1, Settings.Game.Camera.FocusForce * e.ElapsedTimeF);

            this.Focus += (focusGoal - this.Focus) * focusSpeed;

            var positionSpeed = Math.Min(1, Settings.Game.Camera.PositionForce * e.ElapsedTimeF);

            this.Position += (positionGoal - this.Position) * positionSpeed;
        }
    }
}

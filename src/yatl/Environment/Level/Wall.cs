using System;
using OpenTK;

namespace yatl.Environment.Level
{
    sealed class Wall
    {
        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }
        public Vector2 Normal { get; private set; }

        private bool immutable;

        private Wall previous;
        private Wall next;

        public Wall(Vector2 startPoint, Vector2 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.Normal = (endPoint - startPoint).PerpendicularLeft.Normalized();
        }

        #region Mutable Properties

        public Wall Previous
        {
            get { return this.previous; }
            set { this.setMutable(out this.previous, value); }
        }

        public Wall Next
        {
            get { return this.next; }
            set { this.setMutable(out this.next, value); }
        }

        #endregion

        public Wall Frozen { get { this.Freeze(); return this; } }

        private void setMutable<T>(out T field, T value)
        {
            this.assertMutable();
            field = value;
        }

        private void assertMutable()
        {
            if (this.immutable)
                throw new MemberAccessException("Wall is frozen. Members cannot be set anymore.");
        }

        public void Freeze()
        {
            this.immutable = true;
        }
    }
}

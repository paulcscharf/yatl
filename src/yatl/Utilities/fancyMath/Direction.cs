using System;
using OpenTK;

namespace yatl.Utilities
{
    struct Direction : IEquatable<Direction>
    {
        const float fromRadians = ushort.MaxValue / GameMath.TwoPi;
        const float toRadians = GameMath.TwoPi / ushort.MaxValue;

        const float fromDegrees = ushort.MaxValue / 360f;
        const float toDegrees = 360f / ushort.MaxValue;

        private readonly ushort data;

        #region Constructing
		 
        private Direction(ushort data)
        {
            this.data = data;
        }

        public static Direction FromRadians(float radians)
        {
            return new Direction((ushort)(radians * Direction.fromRadians));
        }

        public static Direction FromDegrees(float degrees)
        {
            return new Direction((ushort)(degrees * Direction.fromDegrees));
        }

        public static Direction Of(Vector2 vector)
        {
            return Direction.FromRadians(vector.Angle());
        }

	    #endregion


        #region Static Fields

        public static readonly Direction Zero = new Direction(0);

        #endregion


        #region Properties

        public float Radians { get { return this.data * Direction.toRadians; } }

        public float Degrees { get { return this.data * Direction.toDegrees; } }

        public float RadiansSigned { get { return ((short)this.data) * Direction.toRadians; } }

        public float DegreesSigned { get { return ((short)this.data) * Direction.toDegrees; } }

        public Vector2 Vector { get { return GameMath.Vector2FromRotation(this.Radians); } }

        #endregion


        #region Methods
        


        #endregion


        #region Operators

        #region Arithmetic

        public static Direction operator +(Direction direction, Angle angle)
        {
            return new Direction((ushort)(direction.data + angle.Radians * fromRadians));
        }

        public static Direction operator -(Direction direction, Angle angle)
        {
            return new Direction((ushort)(direction.data - angle.Radians * fromRadians));
        }

        public static Angle operator -(Direction direction1, Direction direction2)
        {
            return Angle.FromRadians(((short)(direction1.data - direction2.data)) * toRadians);
        }

        #endregion

        #region Boolean

        public bool Equals(Direction other)
        {
            return this.data == other.data;
        }

        public override bool Equals(object obj)
        {
            return obj is Direction && this.data == ((Direction)obj).data;
        }
        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }

        public static bool operator ==(Direction x, Direction y)
        {
            return x.data == y.data;
        }
        public static bool operator !=(Direction x, Direction y)
        {
            return x.data != y.data;
        }

        #endregion

        #region Casts



        #endregion

        #endregion

    }
}

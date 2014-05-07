using System;
using OpenTK;
using Matrix2 = amulware.Graphics.Matrix2;

namespace yatl.Utilities
{
    struct Angle : IEquatable<Angle>
    {
        const float fromDegrees = GameMath.TwoPi / 360f;
        const float toDegrees = 360f / GameMath.TwoPi;

        private readonly float radians;

        #region Constructing

        private Angle(float radians)
        {
            this.radians = radians;
        }

        public static Angle FromRadians(float radians)
        {
            return new Angle(radians);
        }

        public static Angle FromDegrees(float degrees)
        {
            return new Angle(degrees * Angle.fromDegrees);
        }

        public static Angle Between(Vector2 from, Vector2 to)
        {
            float perpDot = from.Y * to.X - from.X * to.Y;

            return Angle.FromRadians((float)Math.Atan2(perpDot, Vector2.Dot(from, to)));
        }

        public static Angle Between(Direction from, Direction to)
        {
            return from - to;
        }

	    #endregion


        #region Static Fields

        public static readonly Angle Zero = new Angle(0);

        #endregion


        #region Properties

        public float Radians { get { return this.radians; } }

        public float Degrees { get { return this.radians * Angle.toDegrees; } }

        public Matrix2 Transformation { get { return Matrix2.CreateRotation(this.radians); } }

        public float MagnitudeInRadians { get { return Math.Abs(this.radians); } }

        public float MagnitudeInDegrees { get { return Math.Abs(this.radians * Angle.toDegrees); } }

        #endregion


        #region Methods

        #region Arithmetic

        public float Sin()
        {
            return (float)Math.Sin(this.radians);
        }
        public float Cos()
        {
            return (float)Math.Cos(this.radians);
        }
        public float Tan()
        {
            return (float)Math.Tan(this.radians);
        }
        public int Sign()
        {
            return Math.Sign(this.radians);
        }

        public Angle Abs()
        {
            return new Angle(Math.Abs(this.radians));
        }
        public Angle Normalized()
        {
            return new Angle(Math.Sign(this.radians));
        }

        #endregion

        #endregion


        #region Operators

        #region Arithmetic

        public static Angle operator +(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.radians + angle2.radians);
        }

        public static Angle operator -(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.radians - angle2.radians);
        }

        public static Angle operator *(Angle angle, float scalar)
        {
            return new Angle(angle.radians * scalar);
        }

        public static Angle operator *(float scalar, Angle angle)
        {
            return new Angle(angle.radians * scalar);
        }

        public static Angle operator /(Angle angle, float invScalar)
        {
            return new Angle(angle.radians / invScalar);
        }

        #endregion

        #region Boolean

        public bool Equals(Angle other)
        {
            return this.radians == other.radians;
        }

        public override bool Equals(object obj)
        {
            return obj is Angle && this.radians == ((Angle)obj).radians;
        }
        public override int GetHashCode()
        {
            return this.radians.GetHashCode();
        }

        public static bool operator ==(Angle x, Angle y)
        {
            return x.radians == y.radians;
        }
        public static bool operator !=(Angle x, Angle y)
        {
            return x.radians != y.radians;
        }

        public static bool operator <(Angle x, Angle y)
        {
            return x.radians < y.radians;
        }
        public static bool operator >(Angle x, Angle y)
        {
            return x.radians > y.radians;
        }

        public static bool operator <=(Angle x, Angle y)
        {
            return x.radians <= y.radians;
        }
        public static bool operator >=(Angle x, Angle y)
        {
            return x.radians >= y.radians;
        }

        #endregion

        #region Casts


        #endregion

        #endregion

    }
}

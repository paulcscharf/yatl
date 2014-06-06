using System;
using OpenTK;

namespace yatl.Utilities
{
    static class GameMath
    {
        #region Constants
        /// <summary>
        /// Represents the mathematical constant e (2.71828175).
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Represents the log base ten of e (0.4342945f).
        /// </summary>
        public const float Log10E = 0.4342945f;

        /// <summary>
        /// Represents the log base two of e (1.442695f).
        /// </summary>
        public const float Log2E = 1.442695f;

        /// <summary>
        /// Represents the value of pi (3.14159274).
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// Represents the value of pi divided by two (1.57079637).
        /// </summary>
        public const float PiOver2 = (float)(Math.PI / 2.0);

        /// <summary>
        /// Represents the value of pi divided by four (0.7853982).
        /// </summary>
        public const float PiOver4 = (float)(Math.PI / 4.0);

        /// <summary>
        /// Represents the value of pi times two (6.28318548).
        /// </summary>
        public const float TwoPi = (float)(Math.PI * 2.0);
        #endregion

        #region Methods
        /// <summary>
        /// Clamps the value to a specified range.
        /// </summary>
        /// <param name="value">The value that should be restricted to the specified range.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Clamps the value to a specified range.
        /// </summary>
        /// <param name="value">The value that should be restricted to the specified range.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Clamps the value to a specified range.
        /// </summary>
        /// <param name="value">The value that should be restricted to the specified range.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return value;
        }

        /// <summary>
        /// Perform an addition operation in Zn.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <param name="n">The modulo.</param>
        /// <returns>a + b (mod n)</returns>
        public static int AdditionModuloN(int a, int b, int n)
        {
            return ((a + b) % n + n) % n;
        }

        /// <summary>
        /// Swaps the numbers a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Swaps the numbers a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Swaps the numbers a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Wraps an angle between -pi and pi.
        /// </summary>
        /// <param name="angle">The angle in radius that should be rediuced.</param>
        /// <returns>The angle reduced to an angle between -pi and pi.</returns>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder(angle, Math.PI * 2);

            if (angle <= -Math.PI)
                angle += MathHelper.TwoPi;
            else if (angle > Math.PI)
                angle -= MathHelper.TwoPi;

            return angle;
        }
        #endregion

        #region Interpolations
        /// <summary>
        /// Performs a first order Bezier curve interpolation.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="t">The amount of interpolation (between 0 and 1).</param>
        /// <returns>The result of the Bezier curve interpolation.</returns>
        public static Vector2 Bezier(Vector2 point1, Vector2 point2, float t)
        {
            t = GameMath.Clamp(t, 0, 1);
            return (1 - t) * point1 + t * point2;
        }

        /// <summary>
        /// Performs a second order Bezier curve interpolation.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="t">The amount of interpolation (between 0 and 1).</param>
        /// <returns>The result of the Bezier curve interpolation.</returns>
        public static Vector2 Bezier(Vector2 point1, Vector2 point2, Vector2 point3, float t)
        {
            return GameMath.Bezier(GameMath.Bezier(point1, point2, t), GameMath.Bezier(point2, point3, t), t);
        }

        /// <summary>
        /// Performs a third order Bezier curve interpolation.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="t">The amount of interpolation (between 0 and 1).</param>
        /// <returns>The result of the Bezier curve interpolation.</returns>
        public static Vector2 Bezier(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, float t)
        {
            return GameMath.Bezier(GameMath.Bezier(point1, point2, point3, t), GameMath.Bezier(point2, point3, point4, t), t);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">From position.</param>
        /// <param name="tangent1">From tangent.</param>
        /// <param name="value2">To position.</param>
        /// <param name="tangent2">To tangent.</param>
        /// <param name="amount">The amount of interpolation.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            // All transformed to double not to lose precission.
            // Otherwise, we might get NaN or Infinity result.
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                    (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                    t1 * s +
                    v1;
            return (float)result;
        }

        /// <summary>
        /// Performs a linear interpolation between two values.
        /// </summary>
        /// <param name="from">The first value.</param>
        /// <param name="to">The second value.</param>
        /// <param name="t">The amount of interpolation (between 0 and 1).</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * GameMath.Clamp(t, 0, 1);
        }

        /// <summary>
        /// Performs a biliear interpolation between four values.
        /// Note: The parameters are not clamped to the 0-1 range.
        /// </summary>
        /// <param name="value00">The first value.</param>
        /// <param name="value10">The second value.</param>
        /// <param name="value01">The third value.</param>
        /// <param name="value11">The fourth value.</param>
        /// <param name="u">Parameter in first dimension</param>
        /// <param name="v">Parameter in second dimension.</param>
        /// <returns>The interpolated value.</returns>
        public static float BiLerp(float value00, float value10, float value01, float value11, float u, float v)
        {
            var first = value00 + (value10 - value00) * u;
            var second = value01 + (value11 - value01) * u;
            return first + (second - first) * v;
        }

        /// <summary>
        /// Performs a cubic interpolation between two values.
        /// </summary>
        /// <param name="from">The first value.</param>
        /// <param name="to">The second value.</param>
        /// <param name="t">The amount of interpolation (between 0 and 1).</param>
        /// <returns>The interpolated value.</returns>
        public static float SmoothStep(float from, float to, float t)
        {
            return GameMath.Hermite(from, 0, to, 0, GameMath.Clamp(t, 0, 1));
        }

        public static float AngleLerp(float from, float to, float t)
        {
            from = GameMath.WrapAngle(from);
            to = GameMath.WrapAngle(to);
            if (Math.Abs(from - to) > GameMath.Pi)
            {
                if (from > to)
                    to += GameMath.TwoPi;
                else
                    to -= GameMath.TwoPi;
            }
            return GameMath.Lerp(from, to, t);
        }
        #endregion

        #region Collisions
        /// <summary>
        /// Checks if the point is in a circle of given radius around the origin.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <returns></returns>
        public static bool IsInCircle(Vector2 point, float radius)
        {
            return point.LengthSquared < radius * radius;
        }

        /// <summary>
        /// Checks whether a point lies on a line.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <param name="support">The support point for the line.</param>
        /// <param name="direction">The direction of the line.</param>
        /// <returns>null if there is not intersection. The weight of the direction to the intersection otherwise.</returns>
        public static float? IsOnLine(Vector2 point, Vector2 support, Vector2 direction)
        {
            if (direction.X == 0)
            {
                if (point.X == support.X)
                    return (point.Y - support.Y) / direction.Y;
                else
                    return null;
            }

            float t = (point.X - support.X) / direction.X;

            if (support.Y + direction.Y == point.Y)
                return t;

            return null;
        }

        /// <summary>
        /// Checks if two closed intervals overlap.
        /// </summary>
        /// <param name="amin">The (inclusive) lower bound of the first interval.</param>
        /// <param name="amax">The (inclusive) upper bound of the first interval.</param>
        /// <param name="bmin">The (inclusive) lower bound of the second interval.</param>
        /// <param name="bmax">The (inclusive) upper bound of the second interval.</param>
        /// <returns></returns>
        public static bool IntervalOverlap(double amin, double amax, double bmin, double bmax)
        {
            // Negation of bmin >= amax || amin >= bmax
            return bmin < amax && amin < bmax;
        }
        #endregion

        #region Vector related stuff

        public static Vector3 WithZ(this Vector2 xy, float z)
        {
            return new Vector3(xy.X, xy.Y, z);
        }

        /// <summary>
        /// Creates a vector based on an angle and radius.
        /// </summary>
        /// <param name="angle">The angle the vector should make with the x-axis.</param>
        /// <param name="radius">The length of the vector.</param>
        /// <returns>A vector with the specified rotation and radius.</returns>
        public static Vector2 Vector2FromRotation(float angle, float radius = 1)
        {
            return radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Creates a vector based on two angles and a radius.
        /// </summary>
        /// <param name="radius">The length of the vector.</param>
        /// <returns>A vector with the specified rotation and radius.</returns>
        public static Vector3 Vector3FromRotation(float angle, float angle2, float radius = 1)
        {
            float cos = (float)Math.Cos(angle2);
            return radius * new Vector3(
                (float)Math.Cos(angle) * cos,
                (float)Math.Sin(angle) * cos,
                (float)Math.Sin(angle2)
                );
        }

        /// <summary>
        /// Checks whether a vector is contained in a rectangle.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <param name="topLeft">The top left corner of the rectangle (i.e. the corner with the lowest coordinates).</param>
        /// <param name="bottomRight">The bottom right corner of the rectangle (i.e. the corner with the highest coordinates).</param>
        /// <returns>True if the vector is inside the rectangle.</returns>
        public static bool IsInRectangle(Vector2 point, Vector2 topLeft, Vector2 bottomRight)
        {
            return point.X >= topLeft.X && point.X <= bottomRight.X && point.Y >= topLeft.Y && point.Y <= bottomRight.Y;
        }
        /// <summary>
        /// Checks whether a vector is contained in a rectangle.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <param name="x">The x coordinate of the top left corner (i.e. the corner with the lowest coordinates).</param>
        /// <param name="y">The y coordinate of the top left corner (i.e. the corner with the lowest coordinates).</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>True if the vector is inside the rectangle.</returns>
        public static bool IsInRectangle(Vector2 point, float x, float y, float width, float height)
        {
            return IsInRectangle(point, new Vector2(x, y), new Vector2(x + width, y + height));
        }

        /// <summary>
        /// Calculates the angle of the vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The angle of the vector.</returns>
        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
        #endregion
    }
}

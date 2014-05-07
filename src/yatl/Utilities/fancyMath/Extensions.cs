
namespace yatl.Utilities
{
    static class Extensions
    {
        public static Angle Radians(this float value)
        {
            return Angle.FromRadians(value);
        }
        public static Angle Degrees(this float value)
        {
            return Angle.FromDegrees(value);
        }
    }
}

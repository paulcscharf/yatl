using System;

namespace yatl.Utilities
{
    /// <summary>
    /// Static class to act as global random number generator
    /// Incudes methods for random integers and doubles.
    /// </summary>
    static class GlobalRandom
    {
        private readonly static Random random = new Random();

        /// <summary>
        /// The instance of Random used by GlobalRandom
        /// </summary>
        public static Random Random { get { return random; } }

        /// <summary>
        /// Returns random integer in the interval [0, upper bound[
        /// </summary>
        /// <param name="max">The exclusive upper bound</param>
        public static int Next(int max)
        {
            return random.Next(max);
        }

        /// <summary>
        /// Returns random integer in the interval [lower bound, upper bound[
        /// </summary>
        /// <param name="min">The inclusive lower bound</param>
        /// <param name="max">The exclusive upper bound</param>
        /// <returns></returns>
        public static int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Returns random long integer in the interval [0, upper bound[
        /// </summary>
        /// <param name="max">The exclusive upper bound</param>
        public static long NextLong(long max)
        {
            return GlobalRandom.NextLong(0, max);
        }

        /// <summary>
        /// Returns random long integer in the interval [lower bound, upper bound[
        /// </summary>
        /// <param name="min">The inclusive lower bound</param>
        /// <param name="max">The exclusive upper bound</param>
        /// <returns></returns>
        public static long NextLong(long min, long max)
        {
            var buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }

        /// <summary>
        /// Returns -1 or 1 randomly.
        /// </summary>
        /// <returns></returns>
        public static int NextSign()
        {
            return 2 * random.Next(2) - 1;
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <returns></returns>
        public static bool NextBool()
        {
            return GlobalRandom.NextBool(0.5);
        }

        public static bool NextBool(double probability)
        {
            return GlobalRandom.NextDouble() < probability;
        }

        /// <summary>
        /// Returns random double between 0 and 1
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            return random.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max">The upper bound</param>
        /// <returns></returns>
        public static double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        /// <summary>
        /// Returns a random double between min and max.
        /// </summary>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The upper bound</param>
        /// <returns></returns>
        public static double NextDouble(double min, double max)
        {
            return NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Generates a random double using the standard normal distribution;
        /// </summary>
        /// <returns></returns>
        public static double NormalDouble()
        {
            // Box-Muller
            double u1 = NextDouble();
            double u2 = NextDouble();
            return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        /// <summary>
        /// Generates a random double using the normal distribution with the given mean and deviation.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="deviation"></param>
        /// <returns></returns>
        public static double NormalDouble(double mean, double deviation)
        {
            return mean + deviation * NormalDouble();
        }

        /// <summary>
        /// Returns random double between 0 and 1
        /// </summary>
        /// <returns></returns>
        public static float NextFloat()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Returns a random double between 0 and max.
        /// </summary>
        /// <param name="max">The upper bound</param>
        /// <returns></returns>
        public static float NextFloat(float max)
        {
            return (float)(NextDouble() * max);
        }

        /// <summary>
        /// Returns a random double between min and max.
        /// </summary>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The upper bound</param>
        /// <returns></returns>
        public static float NextFloat(float min, float max)
        {
            return (float)(NextDouble() * (max - min) + min);
        }

        /// <summary>
        /// Returns a random angle(number between 0 and 2pi).
        /// </summary>
        /// <returns></returns>
        public static float Angle()
        {
            return GlobalRandom.NextFloat(GameMath.TwoPi);
        }

        /// <summary>
        /// Generates a random double using the standard normal distribution;
        /// </summary>
        /// <returns></returns>
        public static float NormalFloat()
        {
            // Box-Muller
            double u1 = NextDouble();
            double u2 = NextDouble();
            return (float)(Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2));
        }

        /// <summary>
        /// Generates a random double using the normal distribution with the given mean and deviation.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="deviation"></param>
        /// <returns></returns>
        public static float NormalFloat(float mean, float deviation)
        {
            return mean + (float)(deviation * NormalDouble());
        }
    }
}

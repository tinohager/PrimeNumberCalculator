using System.Numerics;

namespace PrimeNumberCalculator.Helpers
{
    public static class TrialDivisionFilter
    {
        static readonly int[] SmallPrimes =
        {
            3,5,7,11,13,17,19,23,29,31,37,41,43,47
        };

        public static bool PassesTrialDivision(BigInteger n)
        {
            foreach (var p in SmallPrimes)
            {
                if (n % p == 0)
                {
                    return n == p;
                }
            }
            return true;
        }
    }
}

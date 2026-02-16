using System.Numerics;
using System.Security.Cryptography;

namespace PrimeNumberCalculator.Helpers
{
    public static class MillerRabin
    {
        public static bool IsProbablyPrime(BigInteger n, int rounds = 40)
        {
            if (n < 2)
            {
                return false;
            }

            // ignore small prime numbers
            int[] smallPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

            foreach (var p in smallPrimes)
            {
                if (n == p)
                {
                    return true;
                }

                if (n % p == 0)
                {
                    return n == p;
                }
            }

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (int i = 0; i < rounds; i++)
            {
                BigInteger a = RandomBetween(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                {
                    continue;
                }

                bool composite = true;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);

                    if (x == n - 1)
                    {
                        composite = false;
                        break;
                    }
                }

                if (composite)
                {
                    return false;
                }
            }

            return true;
        }

        private static BigInteger RandomBetween(BigInteger min, BigInteger max)
        {
            BigInteger diff = max - min;
            byte[] bytes = diff.ToByteArray();
            BigInteger r;

            using (var rng = RandomNumberGenerator.Create())
            {
                do
                {
                    rng.GetBytes(bytes);
                    bytes[^1] &= 0x7F; // enforce positively
                    r = new BigInteger(bytes);
                }
                while (r > diff);
            }

            return min + r;
        }
    }
}

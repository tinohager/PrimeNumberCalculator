using System.Numerics;

namespace PrimeNumberCalculator.Helpers
{
    public static class NumberHelper
    {
        public static int GetBitLength(BigInteger n)
        {
            if (n.IsZero)
            {
                return 0;
            }

            var bits = n.ToByteArray();
            int lastByte = bits[bits.Length - 1];
            int bitCount = (bits.Length - 1) * 8;
            while (lastByte != 0)
            {
                lastByte >>= 1;
                bitCount++;
            }
            return bitCount;
        }

        public static bool IsPrime(BigInteger n)
        {
            if (n < 2)
            {
                return false;
            }

            if (n == 2)
            {
                return true;
            }

            if (n % 2 == 0)
            {
                return false;
            }

            BigInteger limit = Sqrt(n);

            for (BigInteger i = 3; i <= limit; i += 2)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static BigInteger Sqrt(BigInteger n)
        {
            if (n == 0)
            {
                return 0;
            }

            BigInteger x0 = n / 2;
            BigInteger x1 = (x0 + n / x0) / 2;

            while (x1 < x0)
            {
                x0 = x1;
                x1 = (x0 + n / x0) / 2;
            }

            return x0;
        }
    }
}

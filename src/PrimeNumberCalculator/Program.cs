using PrimeNumberCalculator.Helpers;
using System.Diagnostics;
using System.Numerics;

int threads = Environment.ProcessorCount;
var primeCount = 0;
var stepSize = 1000;

Console.WriteLine($"Using {threads} threads");

var syncLock = new object();
var stopwatch = new Stopwatch();
stopwatch.Start();

BigInteger first512BitNumber = BigInteger.One << 511;

Parallel.For(0, threads, workerId =>
{
    BigInteger number = 2 + workerId + first512BitNumber;

    while (true)
    {
        if (!TrialDivisionFilter.PassesTrialDivision(number))
        {
            number += threads;
            continue;
        }

        bool prime = MillerRabin.IsProbablyPrime(number, 40);

        if (prime)
        {
            var result = Interlocked.Increment(ref primeCount);
            if (result % stepSize == 0)
            {
                lock (syncLock)
                {
                    stopwatch.Stop();
                    Console.Write($"{stopwatch.Elapsed.TotalMilliseconds:0}ms");
                    stopwatch.Restart();
                }

                //int bitCount = (int)Math.Floor(BigInteger.Log(number, 2)) + 1;
                int bitCount = NumberHelper.GetBitLength(number);
                Console.WriteLine($" - Calculate {stepSize} prime numbers with {bitCount} bits");
            }
        }

        number += threads;
    }
});
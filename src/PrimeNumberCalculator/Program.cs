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

BigInteger end = (BigInteger.One << 512) - 1;

static IEnumerable<BigInteger> GetOddNumbers(BigInteger max)
{
    BigInteger first512BitNumber = BigInteger.One << 511;
    if (first512BitNumber.IsEven)
    {
        first512BitNumber++;
    }

    for (BigInteger i = first512BitNumber; i <= max; i += 2)
    {
        yield return i;
    }
}

GetOddNumbers(end)
    .AsParallel()
    .WithDegreeOfParallelism(threads)
    .ForAll(number => {
        if (!TrialDivisionFilter.PassesTrialDivision(number))
        {
            return;
        }

        bool prime = MillerRabin.IsProbablyPrime(number, 40);
        if (!prime)
        {
            return;
        }

        var result = Interlocked.Increment(ref primeCount);
        if (result % stepSize == 0)
        {
            lock (syncLock)
            {
                stopwatch.Stop();
                Console.Write($"{stopwatch.Elapsed.TotalMilliseconds:0}ms");
                stopwatch.Restart();
            }

            int bitCount = NumberHelper.GetBitLength(number);
            Console.WriteLine($" - Calculate {stepSize} prime numbers with {bitCount} bits");
        }
    });
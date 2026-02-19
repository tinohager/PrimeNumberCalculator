using PrimeNumberCalculator.Helpers;
using System.Diagnostics;
using System.Numerics;

double totalElapsedMilliseconds = 0;
int measurementCount = 0;

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
            int bitCount = NumberHelper.GetBitLength(number);

            lock (syncLock)
            {
                stopwatch.Stop();

                totalElapsedMilliseconds += stopwatch.Elapsed.TotalMilliseconds;
                measurementCount++;
                var average = measurementCount == 0 ? 0 : totalElapsedMilliseconds / measurementCount;

                Console.Write($"Calculate {stepSize} prime numbers with {bitCount} bits, ");
                Console.WriteLine($"require {stopwatch.Elapsed.TotalMilliseconds:0}ms, average:{average:0}ms [{measurementCount}]");
                stopwatch.Restart();
            }


            
        }
    });
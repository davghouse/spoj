using System;
using System.Collections;
using System.Collections.Generic;

// Prime Generator
// 2 http://www.spoj.com/problems/PRIME1/
// Returns all the primes between two numbers m and n (inclusive), where 1 <= m <= n <= 1,000,000,000.
public static class PRIME1
{
    private const int _limit = 1000000000;
    private static readonly TrialDivisionDecider _decider;

    static PRIME1()
    {
        _decider = new TrialDivisionDecider(_limit);
    }

    public static IEnumerable<int> Solve(int m, int n)
    {
        for (int i = m; i <= n; ++i)
        {
            if (_decider.IsPrime(i))
            {
                yield return i;
            }
        }
    }
}

public abstract class PrimeDecider
{
    public virtual int Limit { get; protected set; }

    public abstract bool IsPrime(int a);
}

public abstract class PrimeProvider : PrimeDecider
{
    public abstract IReadOnlyList<int> Primes { get; }
}

public sealed class SieveOfEratosthenesDecider : PrimeDecider
{
    private readonly BitArray _sieve;

    public SieveOfEratosthenesDecider(int limit)
    {
        Limit = limit;

        _sieve = new BitArray(Limit + 1, true);
        _sieve[0] = false;
        _sieve[1] = false;

        for (int i = 2; i <= (int)Math.Sqrt(Limit); ++i)
        {
            if (_sieve[i]) // Then i hasn't been sieved yet, so it's prime; sieve its multiples.
            {
                int nextPotentiallyUnsievedMultiple = i * i; // Multiples of i less than this were already sieved from lower primes.
                while (nextPotentiallyUnsievedMultiple <= Limit)
                {
                    _sieve[nextPotentiallyUnsievedMultiple] = false;
                    nextPotentiallyUnsievedMultiple += i; // Room for optimization here; could do += 2i except in the case where i is 2.
                }
            }
        }
    }

    public override bool IsPrime(int a)
        => _sieve[a];
}

public sealed class SieveOfEratosthenesProvider : PrimeProvider
{
    private readonly SieveOfEratosthenesDecider _decider;

    public SieveOfEratosthenesProvider(int limit)
    {
        Limit = limit;
        _decider = new SieveOfEratosthenesDecider(Limit);

        var primes = new List<int>();

        for (int i = 2; i <= Limit; ++i)
        {
            if (_decider.IsPrime(i))
            {
                primes.Add(i);
            }
        }

        Primes = primes.AsReadOnly();
    }

    public override bool IsPrime(int a)
        => _decider.IsPrime(a);

    public override IReadOnlyList<int> Primes { get; }
}

public sealed class TrialDivisionDecider : PrimeDecider
{
    private readonly SieveOfEratosthenesProvider _sieve;

    public TrialDivisionDecider(int limit)
    {
        Limit = limit;
        _sieve = new SieveOfEratosthenesProvider((int)Math.Sqrt(Limit));
    }

    public override bool IsPrime(int a)
    {
        if (a <= _sieve.Limit)
        {
            return _sieve.IsPrime(a);
        }

        foreach (int prime in _sieve.Primes)
        {
            if (prime > Math.Sqrt(a))
                break;

            if (a % prime == 0)
                return false;
        }

        return true;
    }
}

public static class Program
{
    private static void Main()
    {
        int remainingTestCases = int.Parse(Console.ReadLine());

        while (remainingTestCases-- > 0)
        {
            int[] line = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            foreach (int prime in PRIME1.Solve(line[0], line[1]))
            {
                Console.WriteLine(prime);
            }
            Console.WriteLine();
        }
    }
}
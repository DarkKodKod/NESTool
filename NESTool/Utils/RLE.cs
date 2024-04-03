using System.Collections.Generic;
using System.Linq;

namespace NESTool.Utils;

public static class RLE
{
    private static void FlushLiterals(byte? data, ref List<byte> cache, ref List<byte> outputData, int repetitionCount)
    {
        int repeatedValues = repetitionCount;

        byte count = (byte)(cache.Count - repeatedValues);

        outputData.Add(count);

        if (count > 0)
        {
            cache.RemoveRange(count, repeatedValues);

            outputData.AddRange(cache);

            cache.Clear();

            if (data != null)
            {
                for (int i = 0; i < repeatedValues; i++)
                {
                    cache.Add(data.GetValueOrDefault());
                }
            }
        }
    }

    private static void FlushRepetition(ref List<byte> cache, ref List<byte> outputData)
    {
        byte count = (byte)cache.Count;

        outputData.Add(count);
        outputData.Add(cache.Last());

        cache.Clear();
    }

    /// <summary>
    /// This compression is based on a sequense of Literals and Repetition bytes.
    /// The sequese is like this, 
    /// (Number of literals : size 1)(literals : size N) / (Number of literals is 0 if there isnt any)
    /// (Number of repeats : size 1)(repeted value : size 1)
    /// (repeat sequense)
    /// </summary>
    /// <param name="inputData"></param>
    /// <param name="outputData"></param>
    public static void Compress(List<byte> inputData, out List<byte> outputData)
    {
        const byte NullCharacter = 255;

        outputData = new List<byte>();

        List<byte> cache = new List<byte>();
        int repetitionCount = 0;
        bool isRepeting = false;

        foreach (byte data in inputData)
        {
            bool checkLast = cache.Count > 0;

            if (checkLast && data == cache.Last() && repetitionCount < 254)
            {
                cache.Add(data);

                repetitionCount++;

                if (!isRepeting && repetitionCount >= 3)
                {
                    FlushLiterals(data, ref cache, ref outputData, repetitionCount);

                    isRepeting = true;      // now start the repeating process
                }
            }
            else
            {
                // not the same as the previous byte

                if (isRepeting && repetitionCount >= 3)
                {
                    FlushRepetition(ref cache, ref outputData);

                    isRepeting = false;     // now start the literal process
                }

                repetitionCount = 1;

                cache.Add(data);
            }
        }

        if (cache.Count > 0)
        {
            if (!isRepeting)
            {
                FlushLiterals(null, ref cache, ref outputData, 0);
            }
            else
            {
                FlushRepetition(ref cache, ref outputData);
            }
        }

        // end token
        outputData.Add(NullCharacter);
    }
}

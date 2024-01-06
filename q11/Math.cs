namespace q11;

public static class MathQ11
{
    public static int Binomial(int n, int k)
    {
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }

    public static int Factorial(int number)
    {
        if (number == 1)
        {
            return 1;
        }

        return number * (Factorial(number - 1));
    }

    // Enumerate all possible m-size combinations of [0, 1, ..., n-1] array
    // in lexicographic order (first [0, 1, 2, ..., m-1]).
    // https://codereview.stackexchange.com/questions/194967/get-all-combinations-of-selecting-k-elements-from-an-n-sized-array
    // public static IEnumerable<int[]> CombinationsRosettaWoRecursion(int m, int n)
    // {
    //     int[] result = new int[m];
    //     Stack<int> stack = new Stack<int>(m);
    //     stack.Push(0);
    //     while (stack.Count > 0)
    //     {
    //         int index = stack.Count - 1;
    //         int value = stack.Pop();
    //         while (value < n)
    //         {
    //             result[index++] = value++;
    //             stack.Push(value);
    //             if (index != m) continue;
    //             yield return (int[])result.Clone(); // thanks to @xanatos
    //             //yield return result;
    //             break;
    //         }
    //     }
    // }
    //
    // public static IEnumerable<T[]> CombinationsRosettaWoRecursion<T>(T[] array, int m)
    // {
    //     if (array.Length < m)
    //         throw new ArgumentException("Array length can't be less than number of selected elements");
    //     if (m < 1)
    //         throw new ArgumentException("Number of selected elements can't be less than 1");
    //     T[] result = new T[m];
    //     foreach (int[] j in CombinationsRosettaWoRecursion(m, array.Length))
    //     {
    //         for (int i = 0; i < m; i++)
    //         {
    //             result[i] = array[j[i]];
    //         }
    //
    //         yield return result;
    //     }
    // }

    public static IEnumerable<IEnumerable<T>> CombinationsOfK<T>(T[] data, int k)
    {
        int size = data.Length;

        IEnumerable<IEnumerable<T>> Runner(IEnumerable<T> list, int n)
        {
            int skip = 1;
            foreach (var headList in list.Take(size - k + 1).Select(h => new T[] { h }))
            {
                if (n == 1)
                    yield return headList;
                else
                {
                    foreach (var tailList in Runner(list.Skip(skip), n - 1))
                    {
                        yield return headList.Concat(tailList);
                    }
                    skip++;
                }
            }
        }

        return Runner(data, k);
    }
}
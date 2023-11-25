namespace ImageDetectionTests.Client.Extensions;

using System;

public static class EnumerableExtensions
{
    public static List<List<double>> ConvertToMatrix(this string matrixString)
    {
        Console.WriteLine("Input: " + matrixString);
        var result = new List<List<double>>();
        foreach (var line in matrixString.Split("\n"))
        {
            result.Add(line.Split(",").Select(l => double.TryParse(l, out var value) ? value : 0d).ToList());
        }
        Console.WriteLine("Result: " + result.AsJson());
        return result;
    }

    public static T? ByIndex<T>(this IList<T> source, int index) where T : class
    {
        if (source.Count == 0) return null;
        if (index >= source.Count) return null;
        return source[index];
    }

    public static int EnumerableHashCode<T>(this IEnumerable<T> enumerable)
        => enumerable.Aggregate(typeof(T).GetHashCode(), (a, x) => HashCode.Combine(a, x));

    public static IEnumerable<(TFirst First, TSecond Second, TThird Third, TFourth Fourth)> Zip<TFirst, TSecond, TThird, TFourth>(
        this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth)
    {
        using (var e1 = first.GetEnumerator())
        using (var e2 = second.GetEnumerator())
        using (var e3 = third.GetEnumerator())
        using (var e4 = fourth.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext())
            {
                yield return (e1.Current, e2.Current, e3.Current, e4.Current);
            }
        }
    }

    public static List<T> PushIf<T>(this List<T> list, T item, Func<T, bool> condition)
    {
        if (condition(item)) return list.Push(item);
        return list;
    }

    public static List<T> Push<T>(this List<T> list, T item)
    {
        list.Add(item);
        return list;
    }

    public static string Concat(this IEnumerable<object> objects, string separator)
    {
        return string.Join(separator, objects);
    }

    public static bool ChargesAreContinuous(this IEnumerable<float> list) =>
        list.Select((i, j) => i < 1f ? 2 - Math.Round(1 / i) - j : i - j).Distinct().Count() == 1;

    public static bool TryIncrementList(this IList<int> currentList, IList<int> maximumList)
    {
        if (currentList.Sum() >= maximumList.Sum()) return false;
        for (var i = currentList.Count - 1; i >= 0; i--)
        {
            if (currentList[i] == maximumList[i])
            {
                currentList[i] = 0;
            }
            else
            {
                currentList[i]++;
                return true;
            }
        }
        return false;
    }

    public static int NumberOfAddsAndRemovals<T>(this IEnumerable<T> from, IEnumerable<T> toList, Func<T, int> selector)
    {
        var itemCounter = new Dictionary<int, int>();
        foreach (var item in from)
        {
            var key = selector(item);
            if (itemCounter.ContainsKey(key)) itemCounter[key]++; else itemCounter.Add(key, 1);
        }
        foreach (var item in toList)
        {
            var key = selector(item);
            if (itemCounter.ContainsKey(key)) itemCounter[key]--; else itemCounter.Add(key, -1);
        }
        return itemCounter.Sum(x => Math.Abs(x.Value));
    }

    public static IEnumerable<List<T?>> SelectMovingWindowsAround<T>(this IEnumerable<T> values, int windowSize)
    {
        var countBefore = windowSize / 2 - (windowSize % 2 == 0 ? 1 : 0);
        var valueList = values.ToList();
        for (var i = 0; i < valueList.Count; i++)
        {
            var negativeStartIndex = i - countBefore;
            var result = negativeStartIndex < 0 ? Enumerable.Repeat(default(T), -negativeStartIndex).ToList() : new List<T?>();
            var startIndex = Math.Max(negativeStartIndex, 0);
            var itemsToTake = new[] { windowSize, valueList.Count - startIndex, windowSize + negativeStartIndex }.Min();
            result.AddRange(valueList.GetRange(startIndex, itemsToTake));
            result.AddRange(Enumerable.Repeat(default(T), windowSize - result.Count));
            yield return result;
        }
    }

    public static string ConvertListToCommaSeperatedString(this IEnumerable<int> list)
    {
        var result = "";
        foreach (var item in list.OrderBy(l => l).Distinct().ContinuousParts(i => i))
        {
            if (item.Count() == 1) result += item.First() + ",";
            else result += item.First() + "-" + item.Last() + ",";
        }
        return result.TrimEnd(',');
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> list)
    {
        foreach (var item in list) if (item is not null) yield return item;
    }

    public static double Average<T>(this IEnumerable<T>? list, Func<T, double> selector, double defaultReturn)
    {
        return list?.Any() == true ? list.Average(v => selector(v)) : defaultReturn;
    }

    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static IEnumerable<T> SortPepsySynthesisByPosition<T>(this IEnumerable<T> list, Func<T, string?> positionSelect)
    {
        return list.OrderBy(l => positionSelect(l)?[0..2])
                   .ThenBy(l => positionSelect(l)?[4..])
                   .ThenBy(l => positionSelect(l));
    }

    public static IEnumerable<IEnumerable<T>> SplitIntoParts<T>(this IEnumerable<T> list, int parts)
    {
        if (parts <= 0) throw new ArgumentOutOfRangeException("parts must be greater zero");
        var count = list.Count();
        var basePartLength = count / parts;
        var startOfOffsetItems = count - basePartLength * parts;
        var result = new List<IEnumerable<T>>();
        for (var i = 0; i < parts; i++)
        {
            var partLength = basePartLength + (i < startOfOffsetItems ? 1 : 0);
            result.Add(list.Take(partLength));
            list = list.Skip(partLength);
        }
        return result;
    }

    public static IEnumerable<IEnumerable<T>> ContinuousParts<T>(this IEnumerable<T> list,
        Func<T, int> indexSelector)
    {
        var result = new List<List<T>>();
        if (!list.Any()) return result;
        var before = list.First();
        result.Add(new List<T>() { before });
        foreach (var item in list.Skip(1))
        {
            if (indexSelector(item) - indexSelector(before) == 1) result.Last().Add(item);
            else result.Add(new List<T>() { item });
            before = item;
        }
        return result;
    }
}
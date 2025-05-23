﻿namespace Shared.Extensions;

public static class DatabaseChangeExt
{
    public static IDictionary<TKey, TValue> NullIfEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null || !dictionary.Any())
        {
            return null!;
        }
        return dictionary;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T element in source)
        {
            action(element);
        }
        return source;
    }

}

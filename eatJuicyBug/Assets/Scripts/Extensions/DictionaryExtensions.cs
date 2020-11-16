﻿using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static void Increment<TKey>(this Dictionary<TKey, int> dictionary, TKey key)
    {
        dictionary.TryGetValue(key, out int i);
        dictionary[key] = i + 1;
    }

    public static bool Fulfills<TKind, TAmount>(this Dictionary<TKind, TAmount> quantities, Dictionary<TKind, TAmount> requirements) where TAmount : IComparable
    {
        foreach(TKind kind in requirements.Keys)
        {
            quantities.TryGetValue(kind, out TAmount amount);
            if(requirements[kind].CompareTo(amount) > 0)
            {
                return false;
            }
        }

        return true;
    }
}
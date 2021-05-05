using System;
using System.Collections;
using System.Collections.Generic;

public class SelectArray<T> : IEnumerable<T>
{
    private readonly T[] array;
    private readonly int[] after;
    private int count;

    public int Count
    {
        get { return count; }
    }

    private int FirstIndex
    {
        get { return after[array.Length]; }
        set { after[array.Length] = value; }
    }

    public bool IsEmpty
    {
        get { return FirstIndex == array.Length; }
    }

    public SelectArray(T[] ts)
    {
        array = ts;
        after = new int[array.Length + 1];
        Reset();
    }

    public void Reset()
    {
        count = array.Length;
        int i = FirstIndex = 0;
        while(i < array.Length)
        {
            after[i] = ++i;
        }
    }

    public T First()
    {
        if(IsEmpty)
            return default;

        return array[FirstIndex];
    }

    public T SkipFirst()
    {
        if(IsEmpty)
            return default;

        T result = array[FirstIndex];
        FirstIndex = after[FirstIndex];
        count--;
        return result;
    }

    public void SkipWhile(Func<T, bool> predicate)
    {
        while(!IsEmpty && predicate(array[FirstIndex]))
        {
            FirstIndex = after[FirstIndex];
            count--;
        }
    }

    public void TakeWhile(Func<T, bool> predicate)
    {
        count = 0;
        int current = array.Length;
        while(after[current] < array.Length && predicate(array[after[current]]))
        {
            count++;
            current = after[current];
        }

        after[current] = array.Length;
    }

    public void Where(Func<T, bool> predicate)
    {
        int count = 0;
        int current = array.Length;
        while(after[current] < array.Length)
        {
            if(predicate(array[after[current]]))
            {
                current = after[current];
                count++;
            }
            else
            {
                after[current] = after[after[current]];
            }
        }
    }

    public T SkipFirstWhere(Func<T, bool> predicate)
    {
        int current = array.Length;
        while(after[current] < array.Length)
        {
            if(predicate(array[after[current]]))
            {
                T result = array[after[current]];
                after[current] = after[after[current]];
                count--;
                return result;
            }

            current = after[current];
        }

        return default;
    }

    public T SkipFirstWhere(Func<T, bool> predicate, int maxSearchedItems)
    {
        if(maxSearchedItems <= 0)
        {
            return SkipFirstWhere(predicate);
        }

        int searchedItems = 0;
        int current = array.Length;
        while(after[current] < array.Length && searchedItems < maxSearchedItems)
        {
            if(predicate(array[after[current]]))
            {
                T result = array[after[current]];
                after[current] = after[after[current]];
                count--;
                return result;
            }

            current = after[current];
            searchedItems++;
        }

        return default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        int current = FirstIndex;
        while(current < array.Length)
        {
            yield return array[current];
            current = after[current];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

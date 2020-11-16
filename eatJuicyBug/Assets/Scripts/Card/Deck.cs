﻿using System.Collections.Generic;

public class Deck : List<Card>
{
    public Card Draw()
    {
        return DrawNth(0);
    }

    public Card DrawNth(int n)
    {
        if(n < 0 || n >= Count)
            return null;

        Card card = this[Count - 1 - n];
        RemoveAt(Count - 1 - n);
        return card;
    }

    public Card Peek()
    {
        return PeekNth(0);
    }

    public Card PeekNth(int n)
    {
        if(n < 0 || n >= Count)
            return null;

        return this[Count - n - 1];
    }

    public Card Draw(CreatureType type)
    {
        int index = FindLastIndex(card1 => card1 is Creature c && c.Type == type);
        if(index < 0)
            return null;

        Card card2 = this[index];
        RemoveAt(index);
        return card2;
    }
}

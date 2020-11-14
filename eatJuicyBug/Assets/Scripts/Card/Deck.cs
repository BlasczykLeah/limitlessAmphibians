using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();

    public Card Draw()
    {
        if(cards.Count == 0)
            return null;

        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public Card Retrieve(CreatureType type)
    {
        return Retrieve(type, 0, cards.Count);
    }

    public Card Retrieve(CreatureType type, int start, int end)
    {
        end = Math.Min(end, cards.Count);
        for(int i = start; i < end; i++)
        {
            if(cards[i] is Creature c && c.Type == type)
            {
                cards.RemoveAt(i);
                return c;
            }
        }

        return null;
    }
}

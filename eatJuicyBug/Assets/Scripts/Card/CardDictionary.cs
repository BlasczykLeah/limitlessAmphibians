using System.Collections.Generic;
using UnityEngine;

public class CardDictionary : MonoBehaviour
{
    public static CardDictionary instance;

    public Card[] cardPrefs;

    Dictionary<string, Card> cards;

    void Awake()
    {
        instance = this;

        cards = new Dictionary<string, Card>();
        for(int i = 0; i < cardPrefs.Length; i++)
        {
            cards.Add(cardPrefs[i].cardName, cardPrefs[i]);
        }
    }

    public Card GetCard(string name)
    {
        if(cards.ContainsKey(name))
        {
            return cards[name];
        }

        Debug.LogError("invalid card name given: " + name);
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardDictionary : MonoBehaviour
{
    public static CardDictionary instance;

    public GameObject[] cardPrefs;

    Dictionary<string, GameObject> cards;

    void Awake()
    {
        instance = this;

        cards = new Dictionary<string, GameObject>();
        for(int i = 0; i < cardPrefs.Length; i++)
        {
            cards.Add(cardPrefs[i].GetComponent<Card>().cardName, cardPrefs[i]);
        }
    }

    public GameObject GetCard(string name)
    {
        if (cards.ContainsKey(name)) return cards[name];

        Debug.LogError("invalid card name given: " + name);
        return null;
    }
}

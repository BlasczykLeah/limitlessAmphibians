using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardDictionary : MonoBehaviour
{
    public string[] cardNames;
    public GameObject[] cardPrefs;

    Dictionary<string, GameObject> cards;

    void Awake()
    {
        cards = new Dictionary<string, GameObject>();
        for(int i = 0; i < cardPrefs.Length; i++)
        {
            cards.Add(cardNames[i], cardPrefs[i]);
        }
    }

    public GameObject GetCard(string name)
    {
        if (cards.ContainsKey(name)) return cards[name];

        Debug.LogError("invalid card name given");
        return null;
    }
}

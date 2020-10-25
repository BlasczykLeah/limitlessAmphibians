using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayout : MonoBehaviour
{
    Vector3 limitLocalPosition = new Vector3(-0.38F, 0.52F, 0.2F);
    Vector3 winLocalPosition = new Vector3(-0.38F, 0.52F, -0.2F);
    //Vector3 playCardLocalPosition = new Vector3(-0.16F, 0.52F, 0.1F);

    public List<GameObject> tableCards;
    public GameObject limitCard;
    public GameObject winCard;

    void Awake()
    {
        tableCards = new List<GameObject>();
    }

    public void placeCard(GameObject card)
    {
        tableCards.Add(card);
        card.transform.parent = transform;

        if (tableCards.Count > 6)
        {
            Debug.LogWarning("max cards placed, placing at center");
            card.transform.localPosition = new Vector3(0, 0.52F, 0);
        }
        else
        {
            //card.transform.localPosition = playCardLocalPosition;

            //update nextPosition
            //playCardLocalPosition = new Vector3(playCardLocalPosition.x + 0.11F, playCardLocalPosition.y, playCardLocalPosition.z);
        }
    }

    public void removePlacedCard(GameObject card)
    {
        if (!tableCards.Contains(card))
        {
            Debug.LogError(card + " not found here. Cannot remove");
            return;
        }

        //int index = tableCards.IndexOf(card);

        //for(int i = index + 1; i < tableCards.Count; i++)
        //{
            //tableCards[i].transform.localPosition = new Vector3(tableCards[i - 1].transform.localPosition.x - 0.11F, tableCards[i - 1].transform.localPosition.y, tableCards[i - 1].transform.localPosition.z);
        //}
        tableCards.Remove(card);

        //update nextPosition
        //playCardLocalPosition = new Vector3(playCardLocalPosition.x - 0.11F, playCardLocalPosition.y, playCardLocalPosition.z);

        if (Networking.server.host) Networking.server.discardCard(card.GetComponent<CardData>().cardName);
        Destroy(card);
    }

    public void removePlacedCard(string cardName)
    {
        foreach(GameObject c in tableCards)
        {
            if(c.GetComponent<CardData>().cardName == cardName)
            {
                removePlacedCard(c);
                return;
            }
        }
        Debug.LogError("Could not find card to remove. " + cardName);
    }

    public void addLimitCard(GameObject card)
    {
        if (limitCard)
        {
            if(limitCard.GetComponent<CardData>().cardName == card.GetComponent<CardData>().cardName)
            {
                // already have this card
                Destroy(card);
                return;
            }

            if (Networking.server.host) Networking.server.discardCard(limitCard.GetComponent<CardData>().cardName);
            Destroy(limitCard);
        }

        card.transform.parent = transform;
        limitCard = card;
        limitCard.transform.localPosition = limitLocalPosition;
    }

    public void addWinCard(GameObject card)
    {
        if (winCard)
        {
            if (winCard.GetComponent<CardData>().cardName == card.GetComponent<CardData>().cardName)
            {
                // already have this card
                Destroy(card);
                return;
            }

            if (Networking.server.host) Networking.server.discardCard(winCard.GetComponent<CardData>().cardName);
            Destroy(winCard);
        }

        card.transform.parent = transform;
        winCard = card;
        winCard.transform.localPosition = winLocalPosition;
    }
}

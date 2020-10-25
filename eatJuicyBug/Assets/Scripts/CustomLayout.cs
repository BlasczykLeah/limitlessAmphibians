using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayout : MonoBehaviour
{
    Vector3 limitLocalPosition;
    Vector3 winLocalPosition;
    //Vector3 playCardLocalPosition = new Vector3(-0.16F, 0.52F, 0.1F);

    GameObject cardsLayout;
    GameObject winPos;
    GameObject limPos;

    public List<GameObject> tableCards;
    public GameObject limitCard;
    public GameObject winCard;

    void Awake()
    {
        tableCards = new List<GameObject>();
        cardsLayout = transform.GetChild(0).gameObject;
        winPos = transform.GetChild(1).gameObject;
        limPos = transform.GetChild(2).gameObject;

        limitLocalPosition = limPos.transform.localPosition;
        winLocalPosition = winPos.transform.localPosition;
    }

    public void placeCard(GameObject card)
    {
        tableCards.Add(card);
        card.transform.SetParent(cardsLayout.transform);
        card.transform.localPosition = Vector3.zero;

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
        Debug.Log(" trying to remove: " + card);

        if (!tableCards.Contains(card))
        {
            Debug.LogWarning(card + " not found here. Cannot remove");
            return;
        }

        tableCards.Remove(card);

        //if (Networking.server.host)
        //{
            Debug.Log("sending to all others to remove: " + card);
            Networking.server.discardCard(card.GetComponent<CardData>().cardName);
        //}
        Debug.Log("removed: " + card);
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
        Debug.LogWarning("Could not find card to remove. " + cardName);
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

        card.transform.SetParent(transform);
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

        card.transform.SetParent(transform);
        winCard = card;
        winCard.transform.localPosition = winLocalPosition;
    }
}

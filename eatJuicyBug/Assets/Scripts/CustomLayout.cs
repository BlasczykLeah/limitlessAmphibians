using System.Collections.Generic;
using UnityEngine;

public class CustomLayout : MonoBehaviour
{
    [SerializeField] private Transform cardsLayout = null;
    [SerializeField] private Transform winPos = null;
    [SerializeField] private Transform limPos = null;

    public List<Creature> tableCards = new List<Creature>();
    public Limit limitCard;
    public WinCondition winCard;

    public void PlaceCard(Creature card)
    {
        tableCards.Add(card);
        card.transform.SetParent(cardsLayout);
        card.transform.localPosition = Vector3.zero;

        if (tableCards.Count > 6)
        {
            Debug.LogWarning("max cards placed, placing at center");
            card.transform.localPosition = new Vector3(0, 0.52F, 0);
        }
    }

    public void RemovePlacedCard(Creature card)
    {
        Debug.Log(" trying to remove: " + card.name);

        if (!tableCards.Contains(card))
        {
            Debug.LogWarning(card + " not found here. Cannot remove");
            return;
        }

        if (Networking.server.host)
        {
            Networking.server.discardCard(card.cardName);
        }

        tableCards.Remove(card);

        Debug.Log("removed: " + card);
        Destroy(card.gameObject);
    }

    public void RemovePlacedCard(string cardName)
    {
        foreach(Creature c in tableCards)
        {
            if(c.cardName == cardName)
            {
                RemovePlacedCard(c);
                return;
            }
        }
        Debug.LogWarning("Could not find card to remove. " + cardName);
    }

    public void AddLimitCard(Limit card)
    {
        if(limitCard)
        {
            if(limitCard.cardName == card.cardName)
            {
                // already have this card
                Destroy(card.gameObject);
                return;
            }

            if(Networking.server.host)
            {
                Networking.server.discardCard(limitCard.cardName);
            }
            Destroy(limitCard);
        }
        
        card.transform.SetParent(limPos);
        limitCard = card;
        limitCard.transform.localPosition = Vector3.zero;
    }

    public void AddWinCard(WinCondition card)
    {
        if(winCard)
        {
            if(winCard.cardName == card.cardName)
            {
                // already have this card
                Destroy(card.gameObject);
                return;
            }

            if(Networking.server.host)
            {
                Networking.server.discardCard(winCard.cardName);
            }
            Destroy(winCard);
        }

        card.transform.SetParent(winPos);
        winCard = card;
        winCard.transform.localPosition = Vector3.zero;
    }
}

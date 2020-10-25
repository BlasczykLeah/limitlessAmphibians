using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayout : MonoBehaviour
{
    Vector3 limitLocalPosition = new Vector3(-0.38F, 0.52F, 0.2F);
    Vector3 winLocalPosition = new Vector3(-0.38F, 0.52F, -0.2F);
    Vector3 playCardLocalPosition = new Vector3(-0.16F, 0.52F, 0.2F);

    List<GameObject> tableCards;
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

        if (tableCards.Count > 12)
        {
            Debug.LogWarning("max cards placed, placing at center");
            card.transform.localPosition = new Vector3(0, 0.52F, 0);
        }
        else
        {
            card.transform.localPosition = playCardLocalPosition;

            //update nextPosition
            if (tableCards.Count == 6) playCardLocalPosition = new Vector3(-0.16F, playCardLocalPosition.y, -0.1F);
            else playCardLocalPosition = new Vector3(playCardLocalPosition.x + 0.11F, playCardLocalPosition.y, playCardLocalPosition.z);
        }
    }

    public void removePlacedCard(GameObject card)
    {
        if (!tableCards.Contains(card))
        {
            Debug.LogError(card + " not found here. Cannot remove");
            return;
        }

        int index = tableCards.IndexOf(card);

        for(int i = index + 1; i < tableCards.Count; i++)
        {
            tableCards[i].transform.localPosition = tableCards[i - 1].transform.localPosition;
        }
        tableCards.Remove(card);

        //update nextPosition
        if (tableCards.Count == 5) playCardLocalPosition = new Vector3(0.39F, playCardLocalPosition.y, 0.2F);
        else playCardLocalPosition = new Vector3(playCardLocalPosition.x - 0.11F, playCardLocalPosition.y, playCardLocalPosition.z);

        Networking.server.discardCard(card.name);
        Destroy(card);
    }

    public void addLimitCard(GameObject card)
    {
        if (limitCard)
        {
            Networking.server.discardCard(limitCard.name);
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
            Networking.server.discardCard(winCard.name);
            Destroy(winCard);
        }

        card.transform.parent = transform;
        winCard = card;
        winCard.transform.localPosition = winLocalPosition;
    }
}

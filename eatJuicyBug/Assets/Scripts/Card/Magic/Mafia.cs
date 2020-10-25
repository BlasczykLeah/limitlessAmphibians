using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mafia : Magic
{
    // take an opponents card, and put it into your hand
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for (int i = 0; i < GameManager.instance.tableLayouts.Length; i++)
        {
            foreach (GameObject card in GameManager.instance.tableLayouts[i].tableCards)
            {
                if (card.GetComponent<CardData>().GetCreatureType() == creatureType)
                {
                    // if the player has the specific creature type. 
                    GameManager.instance.tableLayouts[i].GetComponent<CustomLayout>().removePlacedCard(card);
                    GameManager.instance.players[i].cardsOnTable--;
                    GameManager.instance.subTractCreature(creatureType, i);
                }
            }
        }
    }
}

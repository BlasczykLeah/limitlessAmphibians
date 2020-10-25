using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warfare : Magic
{

    // destroy each of creature type
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for (int i = 0; i < GameManager.instance.tableLayouts[i].tableCards.Count; i++) {
            if (GameManager.instance.tableLayouts[i].tableCards[i].GetComponent<CardData>().GetCreatureType() == creatureType) {
                // if the player has the specific creature type. 
                GameObject card = GameManager.instance.tableLayouts[i].tableCards[i];
                GameManager.instance.tableLayouts[i].GetComponent<CustomLayout>().removePlacedCard(card);
                GameManager.instance.players[targetIndex].cardsOnTable--;
            }
        }
    }
}

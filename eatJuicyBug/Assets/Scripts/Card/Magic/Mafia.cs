using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mafia : Magic
{
    // take an opponents card, and put it into your hand
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for(int i = 0; i < GameManager.instance.tableLayouts[targetIndex].tableCards.Count; i++) {
            if(GameManager.instance.tableLayouts[targetIndex].tableCards[i].GetComponent<CardData>().GetCreatureType() == creatureType) {
                // if the player has the specific creature type. 
                GameObject card = GameManager.instance.tableLayouts[targetIndex].tableCards[i];
                GameManager.instance.tableLayouts[targetIndex].GetComponent<CustomLayout>().removePlacedCard(card);
                break;
            }
        }
    }
}

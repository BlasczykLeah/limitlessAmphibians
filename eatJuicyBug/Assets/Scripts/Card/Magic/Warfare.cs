using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warfare : Magic
{

    // destroy each of creature type
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for (int i = 0; i < Networking.server.playerSockets.Count; i++) {
            Debug.Log("checking row " + i);
            foreach (GameObject card in GameManager.instance.tableLayouts[i].tableCards) {
                Debug.Log(card.GetComponent<CardData>().cardName);
                if (card.GetComponent<CardData>().GetCreatureType() == creatureType) {
                    // if the player has the specific creature type. 
                    Debug.Log("Removing card: " + card.GetComponent<CardData>().cardName);
                    GameManager.instance.tableLayouts[i].GetComponent<CustomLayout>().removePlacedCard(card);
                    GameManager.instance.players[i].cardsOnTable--;
                    GameManager.instance.subTractCreature(creatureType, i);
                    continue;
                }
            }
        }
    }
}

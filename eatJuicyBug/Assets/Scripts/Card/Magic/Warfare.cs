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
                Debug.Log(card.GetComponent<Card>().cardName);
                if (card.GetComponent<Card>() is Creature c && c.Type == creatureType) {
                    // if the player has the specific creature type. 
                    Debug.Log("Removing card: " + card.GetComponent<Card>().cardName);
                    GameManager.instance.tableLayouts[i].GetComponent<CustomLayout>().removePlacedCard(card);

                    Debug.Log("does it get here?");
                    GameManager.instance.players[i].cardsOnTable--;
                    GameManager.instance.subTractCreature(creatureType, i);
                    Debug.Log("or here?");
                    break;
                }
            }
        }
    }
}

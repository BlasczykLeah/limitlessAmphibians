using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MafiaMagic : Magic
{
    // take an opponents card, and put it into your hand
    
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for (int i = 0; i < GameManager.instance.tableLayouts[targetIndex].tableCards.Count; i++) {
            if (GameManager.instance.tableLayouts[targetIndex].tableCards[i].GetComponent<Card>() is Creature c && c.Type == creatureType) {
                // if the player has the specific creature type. 
                Creature card = GameManager.instance.tableLayouts[targetIndex].tableCards[i];
                GameManager.instance.tableLayouts[targetIndex].GetComponent<CustomLayout>().RemovePlacedCard(card);
                GameManager.instance.players[targetIndex].cardsOnTable--;
                GameManager.instance.subTractCreature(creatureType, playerIndex);
                return;
            }
        }
    }
}

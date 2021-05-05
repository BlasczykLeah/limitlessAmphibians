using UnityEngine;

public class WarfareMagic : Magic
{

    // destroy each of creature type
    public override void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {
        for (int i = 0; i < Networking.server.playerSockets.Count; i++) {
            Debug.Log("checking row " + i);
            foreach(Creature card in GameManager.instance.tableLayouts[i].tableCards) {
                Debug.Log(card.cardName);
                if (card.Type == creatureType) {
                    // if the player has the specific creature type. 
                    Debug.Log("Removing card: " + card.cardName);
                    GameManager.instance.tableLayouts[i].GetComponent<CustomLayout>().RemovePlacedCard(card);

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

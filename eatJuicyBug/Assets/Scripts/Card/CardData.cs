using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public string cardName;

    [SerializeField] CardType type = CardType.Creature;
    [SerializeField] CreatureType c_type = CreatureType.None;

    public int playerIndex = -1;

    public CardType GetCardType() {
        return type;
    }

    public CreatureType GetCreatureType()
    {
        return c_type;
    }

    public Limit limit;
    public Magic magic;
    //Add as many effects into a card as you would like
    public List<CardEffect> cardEffects = new List<CardEffect>();

    public void Play() {
        if (GameManager.instance.players[0].limit.Permits(type, c_type)) { // checks limit
            for (int i = 0; i < cardEffects.Count; i++) {
                if (type == CardType.Creature) {
                    cardEffects[i].DetermineCreatureType(c_type);
                }
                cardEffects[i].PlayCard();
            }
        } else {
            print("Cannot play this card.");
        }
    }
}

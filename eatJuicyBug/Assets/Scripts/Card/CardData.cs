using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public string cardName;

    [SerializeField] CardType type;
    [SerializeField] CreatureType c_type;

    public CardType GetCardType() {
        return type;
    }

    public CreatureType GetCreatureType()
    {
        return c_type;
    }

    public Limit limit;
    //Add as many effects into a card as you would like
    public List<CardEffect> cardEffects = new List<CardEffect>();

    public void Play() {
        if (GameManager.instance.players[0].limit.CheckLimit(type, c_type)) { // checks limit
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

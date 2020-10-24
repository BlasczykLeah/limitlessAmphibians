using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [SerializeField] CardType type;
    [SerializeField] CreatureType c_type;

    public CardType GetCardType() {
        return type;
    }

    //Add as many effects into a card as you would like
    public List<CardEffect> cardEffects = new List<CardEffect>();

    public void Play() {
        for(int i = 0; i < cardEffects.Count; i++) {
            if(type == CardType.Creature) {  
                cardEffects[i].DetermineCreatureType(c_type);
            }
            cardEffects[i].PlayCard();
        }
    }
}

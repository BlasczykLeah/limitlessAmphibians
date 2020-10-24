using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    //Add as many effects into a card as you would like
    public List<CardEffect> cardEffects = new List<CardEffect>();

    public void Play() {
        for(int i = 0; i < cardEffects.Count; i++) {
            cardEffects[i].PlayCard();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static Deck instance;
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public List<Card> cards = new List<Card>();
    public List<WinCondition> winConditions = new List<WinCondition>();
}

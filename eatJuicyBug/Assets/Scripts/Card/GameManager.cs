using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public List<Player> players = new List<Player>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }



    }

    private void Start()
    {
        Networking.server.getPlayers();
    }

    //you draw a card...yay
    public void DrawCard() {
        //Card card = Deck.instance.cards[Deck.instance.cards.Count - 1];    
    }

    // ALL CARD FUNCTIONS
    public void PlayCreature(CreatureType type) {

        print("I am playing a " + type.ToString());

    }

    public void PlayLimit() {
        print("I am playing a limit");
    }

    public void PlaySwap() {

    }

    public void PlayDestroy() {

    }

    
}

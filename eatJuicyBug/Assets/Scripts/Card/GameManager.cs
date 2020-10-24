using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public List<Player> players = new List<Player>();

    public bool ready = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        Invoke("GetPlayers", 2);
    }

    void Update()
    {
        if (ready)
        {
            if (GetComponent<PlayerViews>()) GetComponent<PlayerViews>().enableButtons(players.Count);
            ready = false;
        }
    }

    void GetPlayers() {
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

    public void PlayLimit(Limit limit) {

        int num = Random.Range(0, players.Count);
        players[num].limit = limit;

    }

    public void PlaySwap() {

    }

    public void PlayDestroy() {

    }

    
}

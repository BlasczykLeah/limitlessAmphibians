﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public List<Player> players = new List<Player>();
    public CustomLayout[] tableLayouts;
    public GameObject playerHand;

    public TextMeshProUGUI turnText;

    int me = -1;
    public int turn;
    public bool ready = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        Networking.server.getPlayers();
    }

    private void StartGame() {
        //turn = 0;
        //print("It is Player " + turn + "'s turn!");

        //Networking.server.EnableNextTurn();
    }
    public void NextTurn() {
        if(players[turn].creatureAmounts[0] >= players[turn].winCon.frog &&
           players[turn].creatureAmounts[1] >= players[turn].winCon.dragon &&
           players[turn].creatureAmounts[2] >= players[turn].winCon.gator &&
           players[turn].creatureAmounts[3] >= players[turn].winCon.azolotl &&
           players[turn].creatureAmounts[4] >= players[turn].winCon.lizard &&
           players[turn].creatureAmounts[5] >= players[turn].winCon.dino) {

            //ta-da!!!
            WinGame();
        } else {
            //turn++;
            //if(turn >= players.Count - 1) {
            //    turn = 0;
            //}
            print("It is Player " + turn + "'s turn!");
        }
    }

    void WinGame() {
        print("WINNER: Player " + turn);
    }
    void Update()
    {
        if (ready)
        {
            if (GetComponent<PlayerViews>()) GetComponent<PlayerViews>().enableButtons(players.Count);

            Invoke("StartGame", 2f);
            ready = false;
        }
    }

    //you draw a card...yay
    public void DrawCard() {
        //Card card = Deck.instance.cards[Deck.instance.cards.Count - 1];    
    }

    // ALL CARD FUNCTIONS
    public void PlayCreature(CreatureType type, int index) {
        switch (type) {
            case CreatureType.Frog:
                players[index].creatureAmounts[0]++;
                break;
            case CreatureType.Dragon:
                players[index].creatureAmounts[1]++;
                break;
            case CreatureType.Gator:
                players[index].creatureAmounts[2]++;
                break;
            case CreatureType.Axolotl:
                players[index].creatureAmounts[3]++;
                break;
            case CreatureType.Lizard:
                players[index].creatureAmounts[4]++;
                break;
            case CreatureType.Dino:
                players[index].creatureAmounts[5]++;
                break;
        }
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

    public void InstantiateMyCards(int myIndex, GameObject[] cardPrefs, GameObject limPref)
    {
        me = myIndex;
        GetComponent<PlayerViews>().setView(me);

        foreach(GameObject card in cardPrefs)
        {
            GameObject newCard = Instantiate(card, playerHand.transform);
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
            players[me].Hand.Add(newCard.GetComponent<Card>());
            //tableLayouts[me].placeCard(newCard);
            
        }

        GameObject newLimit = Instantiate(limPref);
        players[me].limit = newLimit.GetComponent<Limit>();
        tableLayouts[me].addLimitCard(newLimit);
    }

    public int GetPlayerIndexFromID(string id)
    {
        foreach (Player p in players)
        {
            if (p.id == id) return players.IndexOf(p);
        }

        Debug.LogError(id + " not found.");
        return -1;
    }

    public void setTurn(int index)
    {
        turn = index;
        turnText.text = players[index].name + "'s Turn";
    }

    public void playCard(GameObject card)
    {
        if (turn == me)
        {
            //can play
            card.GetComponent<CardClicker>().played = true;
            Networking.server.playCard(card.GetComponent<CardData>().cardName);
        }
        else
        {
            //can't play
            Debug.LogError("Its not your turn!");
            card.GetComponent<CardClicker>().played = false;
        }
    }
}

using System.Collections;
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

    float timer = 5F;
    public GameObject skipButton;

    public GameObject targetBox; 
    public TextMeshProUGUI chooseTargetText;
    Creature cardPlayed_selfCreature = null;
    Card cardPlayed_playerTarget = null;
    Card cardPlayed_otherCreature = null;
    public GameObject[] playerTargetButtons;

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
        if(players[turn].creatureAmounts.Fulfills(players[turn].winCon.requirements)) {
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
        Networking.server.sendWin();
    }
    void Update()
    {
        if (ready)
        {
            if (GetComponent<PlayerViews>()) GetComponent<PlayerViews>().enableButtons(players.Count);

            Invoke("StartGame", 2f);
            ready = false;
        }

        if (turn == me && !cardPlayed_selfCreature)
        {
            timer -= Time.deltaTime;
            if (timer < 0 && !skipButton.activeInHierarchy) skipButton.SetActive(true);
        }

        if (cardPlayed_selfCreature)    // targeting one of my own creatures
        {
            if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if (Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if (raycastStuff.collider.GetComponent<Card>() is Creature c)
                {
                    chooseTargetText.text = "Select this card";
                    if (Input.GetMouseButtonDown(0) && c.playerIndex == me)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        //tableLayouts[me].removePlacedCard(raycastStuff.collider.gameObject);
                        //players[me].cardsOnTable--;

                        players[turn].myTurn = false;
                        Networking.server.playCard(cardPlayed_selfCreature.GetComponent<Card>().cardName, players[me].id, c.cardName);
                        targetBox.SetActive(false);
                        cardPlayed_selfCreature = null;
                    }
                }
                else chooseTargetText.text = "Choose a card to destroy";
            }
            else
            {
                chooseTargetText.text = "Choose a card to destroy";
            }
        }

        if (cardPlayed_otherCreature)   // targeting someone else's creature
        {
            if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if (Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if (raycastStuff.collider.GetComponent<Card>() is Creature c)
                {
                    chooseTargetText.text = "Select this card";
                    if (Input.GetMouseButtonDown(0) && c.playerIndex != me && c.playerIndex != -1)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        //tableLayouts[raycastStuff.collider.GetComponent<CardData>().playerIndex].removePlacedCard(raycastStuff.collider.gameObject);

                        string playerSelected = players[c.playerIndex].id;

                        players[turn].myTurn = false;
                        Networking.server.playCard(cardPlayed_selfCreature.GetComponent<Card>().cardName, playerSelected, c.cardName);
                        targetBox.SetActive(false);
                        cardPlayed_otherCreature = null;
                    }
                }
                else chooseTargetText.text = "Choose another player's card to destroy";
            }
            else
            {
                chooseTargetText.text = "Choose another player's card to destroy";
            }
        }
    }

    //you draw a card...yay
    public void DrawCard() {
        //Card card = Deck.instance.cards[Deck.instance.cards.Count - 1];    
    }

    // ALL CARD FUNCTIONS
    public void PlayCreature(CreatureType type, int index) {
        //players[index].cardsOnTable++;
        players[index].creatureAmounts.Increment(type);
        print("I am playing " + (type == CreatureType.Axolotl ? "an " : "a ") + type.ToString());

    }

    public void PlayLimit(Limit limit, int index) {

        players[index].limit = limit;

        tableLayouts[index].AddLimitCard(limit);
        //Networking.server.updateValues(limit.gameObject.GetComponent<CardData>().cardName, players[index].winCon.GetComponent<CardData>().cardName);
    }

    public void PlaySwap() {

    }

    public void PlayDestroy() {

    }

    public void InstantiateMyCards(int myIndex, Card[] cardPrefs, Limit limPref, WinCondition winPref)
    {
        me = myIndex;
        GetComponent<PlayerViews>().setView(me);

        foreach(Card card in cardPrefs)
        {
            Card newCard = Instantiate(card, playerHand.transform);
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
            players[me].hand.Add(newCard);
            //tableLayouts[me].placeCard(newCard);
            
        }

        Limit newLimit = Instantiate(limPref);
        newLimit.GetComponent<CardClicker>().played = true;
        players[me].limit = newLimit.GetComponent<Limit>();
        tableLayouts[me].AddLimitCard(newLimit);

        WinCondition newWin = Instantiate(winPref);
        newWin.GetComponent<CardClicker>().played = true;
        players[me].winCon = newWin.GetComponent<WinCondition>();
        tableLayouts[me].AddWinCard(newWin);
    }

    public void DrawCard(Card cardPref)
    {
        Card newCard = Instantiate(cardPref, playerHand.transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
        players[me].hand.Add(newCard.GetComponent<Card>());
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
        players[turn].myTurn = true;
        turnText.text = players[index].name + "'s Turn";

        timer = 5F;
        skipButton.SetActive(false);

        GetComponent<PlayerViews>().setView(turn);
    }

    public void playCard(Card card)
    {
        if(turn != me || !players[me].myTurn)
        {
            //can't play
            Debug.LogWarning("Its not your turn!");
            card.GetComponent<CardClicker>().played = false;
            return;
        }

        if(!players[me].limit.Permits(card.GetComponent<Card>()))
        {
            Debug.LogWarning("LIMIT WORKED");
            return;
        }

        switch(card)
        {
            case Creature c:
                if(players[me].cardsOnTable < 6)
                {
                    //can play
                    skipButton.SetActive(false);
                    players[turn].myTurn = false;
                    card.GetComponent<CardClicker>().played = true;
                    Networking.server.playCard(card.cardName);
                }
                else
                {
                    // need to remove card from table
                    // show which card is to be played
                    cardPlayed_selfCreature = c;
                    targetBox.SetActive(true);
                }
                break;
            case Limit _:
                skipButton.SetActive(false);
                cardPlayed_playerTarget = card;
                enableTargetButtons(true);
                break;
            case Magic m when m.target == MagicTarget.All:
                skipButton.SetActive(false);
                players[turn].myTurn = false;
                card.GetComponent<CardClicker>().played = true;
                Networking.server.playCard(card.cardName);
                break;
            case Magic m when m.target == MagicTarget.Opponent:
                skipButton.SetActive(false);
                cardPlayed_playerTarget = card;
                enableTargetButtons(true);
                break;
            case Magic m when m.target == MagicTarget.Card:
                cardPlayed_otherCreature = card;
                targetBox.SetActive(true);
                break;
        }
    }

    public void skipTurn()
    {
        if (turn == me && players[me].myTurn)
        {
            players[turn].myTurn = false;
            skipButton.SetActive(false);
            Networking.server.playCard("none");
        }
    }

    void enableTargetButtons(bool enable)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (!players[i].disconnected) playerTargetButtons[i].SetActive(enable);
        }
    }

    public void choosePlayer(int index)
    {
        players[turn].myTurn = false;
        cardPlayed_playerTarget.GetComponent<CardClicker>().played = true;
        Networking.server.playCard(cardPlayed_playerTarget.GetComponent<Card>().cardName, players[index].id);

        enableTargetButtons(false);
        cardPlayed_playerTarget = null;
    }

    public void subTractCreature(CreatureType type, int index)
    {
        players[index].creatureAmounts[type]--;
    }
}

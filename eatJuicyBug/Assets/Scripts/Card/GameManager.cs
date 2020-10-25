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
    GameObject cardToPlay = null;

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

        if (turn == me && !cardToPlay)
        {
            timer -= Time.deltaTime;
            if (timer < 0 && !skipButton.activeInHierarchy) skipButton.SetActive(true);
        }

        if (cardToPlay)
        {
            if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit thing;
            if (Physics.Raycast(ray, out thing, 30, LayerMask.GetMask("Card")))
            {
                if (thing.collider.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    chooseTargetText.text = "Select this card";
                    if (Input.GetMouseButtonDown(0) && thing.collider.GetComponent<CardClicker>().played)
                    {
                        // card chosen
                        tableLayouts[me].removePlacedCard(thing.collider.gameObject);

                        players[turn].myTurn = false;
                        Networking.server.playCard(cardToPlay.GetComponent<CardData>().cardName);
                        targetBox.SetActive(false);
                        cardToPlay = null;
                    }
                }
                else chooseTargetText.text = "Choose a card to destroy";
            }
            else
            {
                chooseTargetText.text = "Choose a card to destroy";
            }
        }
    }

    //you draw a card...yay
    public void DrawCard() {
        //Card card = Deck.instance.cards[Deck.instance.cards.Count - 1];    
    }

    // ALL CARD FUNCTIONS
    public void PlayCreature(CreatureType type, int index) {
        players[index].cardsOnTable++;
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

    public void PlayLimit(Limit limit, int index) {

        players[index].limit = limit;

        tableLayouts[index].addLimitCard(limit.gameObject);
        //Networking.server.updateValues(limit.gameObject.GetComponent<CardData>().cardName, players[index].winCon.GetComponent<CardData>().cardName);
    }

    public void PlaySwap() {

    }

    public void PlayDestroy() {

    }

    public void InstantiateMyCards(int myIndex, GameObject[] cardPrefs, GameObject limPref, GameObject winPref)
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
        newLimit.GetComponent<CardClicker>().played = true;
        players[me].limit = newLimit.GetComponent<Limit>();
        tableLayouts[me].addLimitCard(newLimit);

        GameObject newWin = Instantiate(winPref);
        newWin.GetComponent<CardClicker>().played = true;
        players[me].winCon = newWin.GetComponent<WinCondition>();
        tableLayouts[me].addWinCard(newWin);
    }

    public void DrawCard(GameObject cardPref)
    {
        GameObject newCard = Instantiate(cardPref, playerHand.transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
        players[me].Hand.Add(newCard.GetComponent<Card>());
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

    public void playCard(GameObject card)
    {
        if (turn == me && players[me].myTurn)
        {
            if (players[me].limit.CheckLimit(card.GetComponent<CardData>().GetCardType(), card.GetComponent<CardData>().GetCreatureType()))
            {
                if (players[me].cardsOnTable < 6 || card.GetComponent<CardData>().GetCardType() != CardType.Creature)
                {
                    //can play
                    skipButton.SetActive(false);
                    players[turn].myTurn = false;
                    card.GetComponent<CardClicker>().played = true;
                    Networking.server.playCard(card.GetComponent<CardData>().cardName);
                }
                else
                {
                    // need to remove card from table
                    // show which card is to be played
                    cardToPlay = card;
                    targetBox.SetActive(true);
                }
            }
            else Debug.LogError("LIMIT WORKED");
        }
        else
        {
            //can't play
            Debug.LogError("Its not your turn!");
            card.GetComponent<CardClicker>().played = false;
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
}

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
    GameObject cardPlayed_selfCreature = null;
    GameObject cardPlayed_playerTarget = null;
    GameObject cardPlayed_otherCreature = null;
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

        if (cardPlayed_selfCreature)    // targetting one of my own creatures
        {
            if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if (Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if (raycastStuff.collider.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    chooseTargetText.text = "Select this card";
                    if (Input.GetMouseButtonDown(0) && raycastStuff.collider.GetComponent<CardData>().playerIndex == me)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        tableLayouts[me].removePlacedCard(raycastStuff.collider.gameObject);
                        players[me].cardsOnTable--;

                        players[turn].myTurn = false;
                        Networking.server.playCard(cardPlayed_selfCreature.GetComponent<CardData>().cardName);
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

        if (cardPlayed_otherCreature)   // targetting someone else's creature
        {
            if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if (Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if (raycastStuff.collider.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    chooseTargetText.text = "Select this card";
                    if (Input.GetMouseButtonDown(0) && raycastStuff.collider.GetComponent<CardData>().playerIndex != me && raycastStuff.collider.GetComponent<CardData>().playerIndex != -1)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        //tableLayouts[raycastStuff.collider.GetComponent<CardData>().playerIndex].removePlacedCard(raycastStuff.collider.gameObject);

                        string playerSelected = players[raycastStuff.collider.GetComponent<CardData>().playerIndex].id;

                        players[turn].myTurn = false;
                        Networking.server.playCard(cardPlayed_selfCreature.GetComponent<CardData>().cardName, playerSelected, raycastStuff.collider.GetComponent<CardData>().cardName);
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
                if (players[me].cardsOnTable < 6 && card.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    //can play
                    skipButton.SetActive(false);
                    players[turn].myTurn = false;
                    card.GetComponent<CardClicker>().played = true;
                    Networking.server.playCard(card.GetComponent<CardData>().cardName);
                }
                else if(card.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    // need to remove card from table
                    // show which card is to be played
                    cardPlayed_selfCreature = card;
                    targetBox.SetActive(true);
                }
                else if(card.GetComponent<CardData>().GetCardType() == CardType.Limit)
                {
                    skipButton.SetActive(false);
                    cardPlayed_playerTarget = card;
                    enableTargetButtons(true);
                }
                else
                {
                    //magic: Magic_Target, Magic_CardTarget, Magic_All
                    if(card.GetComponent<CardData>().GetCardType() == CardType.Magic_All)
                    {
                        skipButton.SetActive(false);
                        players[turn].myTurn = false;
                        card.GetComponent<CardClicker>().played = true;
                        Networking.server.playCard(card.GetComponent<CardData>().cardName);
                    }
                    else if(card.GetComponent<CardData>().GetCardType() == CardType.Magic_Target)
                    {
                        skipButton.SetActive(false);
                        cardPlayed_playerTarget = card;
                        enableTargetButtons(true);
                    }
                    else    // Magic_CardTarget
                    {
                        cardPlayed_otherCreature = card;
                        targetBox.SetActive(true);
                    }
                }
            }
            else Debug.LogWarning("LIMIT WORKED");
        }
        else
        {
            //can't play
            Debug.LogWarning("Its not your turn!");
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
        Networking.server.playCard(cardPlayed_playerTarget.GetComponent<CardData>().cardName, players[index].id);

        enableTargetButtons(false);
        cardPlayed_playerTarget = null;
    }

    public void subTractCreature(CreatureType type, int index)
    {
        switch (type)
        {
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
    }
}

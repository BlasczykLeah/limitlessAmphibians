using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;
    public List<Player> players = new List<Player>();
    public CustomLayout[] tableLayouts;
    public GameObject playerHand;

    public TextMeshProUGUI turnText;

    int me = -1;
    public int turn;
    //public bool ready = false;

    //float timer = 5F;
    //public GameObject skipButton;

    public GameObject targetBox; 
    public TextMeshProUGUI chooseTargetText;
    Creature cardPlayed_selfCreature = null;
    Card cardPlayed_playerTarget = null;
    Card cardPlayed_otherCreature = null;
    public GameObject[] playerTargetButtons;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        PlayerViews views = GetComponent<PlayerViews>();
        if(views is null)
        {
            return;
        }

        for(int i = 0; i < players.Count; i++)
        {
            views.viewButtons[i].SetActive(true);
            views.viewButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = players[i].name;
        }

        GameObject limitCard = CardDictionary.instance.GetCard("Limit1");
        GameObject winCard = CardDictionary.instance.GetCard("Win1");
        GameObject[] newHand = new GameObject[] {
            CardDictionary.instance.GetCard("Axolotl1"),
            CardDictionary.instance.GetCard("Dino1"),
            CardDictionary.instance.GetCard("Dragon1"),
            CardDictionary.instance.GetCard("Frog1"),
            CardDictionary.instance.GetCard("Gator1"),
            CardDictionary.instance.GetCard("Lizard1") };

        InstantiateMyCards(0, newHand, limitCard, winCard);
    }

    public void NextTurn()
    {
        if(players[turn].creatureAmounts.Fulfills(players[turn].winCon.requirements)) {
            WinGame();
        } else {
            print("It is Player " + turn + "'s turn!");
        }
    }

    void WinGame()
    {
        print("WINNER: Player " + turn);
    }

    void Update()
    {
        /*
        if(turn == me && !cardPlayed_selfCreature)
        {
            timer -= Time.deltaTime;
            if(timer < 0 && !skipButton.activeInHierarchy)
            {
                skipButton.SetActive(true);
            }
        }
        */

        if(cardPlayed_selfCreature)
        {
            /*
            if(skipButton.activeInHierarchy)
            {
                skipButton.SetActive(false);
            }
            */

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if(Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if(raycastStuff.collider.GetComponent<Card>() is Creature c)
                {
                    chooseTargetText.text = "Select this card";
                    if(Input.GetMouseButtonDown(0) && c.playerIndex == me)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        //tableLayouts[me].removePlacedCard(raycastStuff.collider.gameObject);
                        //players[me].cardsOnTable--;

                        players[turn].myTurn = false;
                        //Networking.server.playCard(cardPlayed_selfCreature.GetComponent<CardData>().cardName, players[me].id, raycastStuff.collider.GetComponent<CardData>().cardName);
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

        if(cardPlayed_otherCreature)   // targetting someone else's creature
        {
            //if (skipButton.activeInHierarchy) skipButton.SetActive(false);

            //raycast, looking for deletable card
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastStuff;
            if(Physics.Raycast(ray, out raycastStuff, 30, LayerMask.GetMask("Card")))
            {
                if(raycastStuff.collider.GetComponent<Card>() is Creature c)
                {
                    chooseTargetText.text = "Select this card";
                    if(Input.GetMouseButtonDown(0) && c.playerIndex != me && c.playerIndex != -1)   //thing.collider.GetComponent<CardClicker>().played
                    {
                        // card chosen
                        //tableLayouts[raycastStuff.collider.GetComponent<CardData>().playerIndex].removePlacedCard(raycastStuff.collider.gameObject);

                        string playerSelected = players[c.playerIndex].id;

                        players[turn].myTurn = false;
                        //Networking.server.playCard(cardPlayed_selfCreature.GetComponent<CardData>().cardName, playerSelected, raycastStuff.collider.GetComponent<CardData>().cardName);
                        targetBox.SetActive(false);
                        cardPlayed_otherCreature = null;
                    }
                }
                else
                {
                    chooseTargetText.text = "Choose another player's card to destroy";
                }
            }
            else
            {
                chooseTargetText.text = "Choose another player's card to destroy";
            }
        }
    }

    public void DrawCard()
    {
        //Card card = Deck.instance.cards[Deck.instance.cards.Count - 1];    
    }

    // ALL CARD FUNCTIONS
    public void PlayCreature(CreatureType type, int index)
    {
        //players[index].cardsOnTable++;
        players[index].creatureAmounts.Increment(type);
        print("I am playing " + (type == CreatureType.Axolotl ? "an " : "a ") + type.ToString());
    }

    public void PlayLimit(Limit limit, int index)
    {
        players[index].limit = limit;

        tableLayouts[index].addLimitCard(limit.gameObject);
    }

    public void PlaySwap()
    {
    }

    public void PlayDestroy()
    {
    }

    public void InstantiateMyCards(int myIndex, GameObject[] cardPrefs, GameObject limPref, GameObject winPref)
    {
        me = myIndex;
        GetComponent<PlayerViews>().setView(me);

        GameObject newLimit = Instantiate(limPref);
        newLimit.GetComponent<CardClicker>().played = true;
        players[me].limit = newLimit.GetComponent<Limit>();
        tableLayouts[me].addLimitCard(newLimit);

        GameObject newWin = Instantiate(winPref);
        newWin.GetComponent<CardClicker>().played = true;
        players[me].winCon = newWin.GetComponent<WinCondition>();
        tableLayouts[me].addWinCard(newWin);

        for(int i = 0; i < cardPrefs.Length; i++)
        {
            GameObject newCard = Instantiate(cardPrefs[i], playerHand.transform);
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
            newCard.GetComponent<SpriteRenderer>().sortingOrder = i;
            newCard.GetComponent<CardClicker>().hoverEffectEnabled = true;
            if(!players[me].limit.Permits(newCard.GetComponent<Card>()))
            {
                newCard.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            }
            players[me].hand.Add(newCard.GetComponent<Card>());
        }
    }

    public void DrawCard(GameObject cardPref)
    {
        GameObject newCard = Instantiate(cardPref, playerHand.transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
        players[me].hand.Add(newCard.GetComponent<Card>());
    }

    public int GetPlayerIndexFromID(string id)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].id == id)
            {
                return i;
            }
        }

        Debug.LogError(id + " not found.");
        return -1;
    }

    public void SetTurn(int index)
    {
        turn = index;
        players[turn].myTurn = true;
        turnText.text = players[index].name + "'s Turn";

        //timer = 5F;
        //skipButton.SetActive(false);

        GetComponent<PlayerViews>().setView(turn);
    }

    public void PlayCard(Card card)
    {
        if(turn != me || !players[me].myTurn)
        {
            //can't play
            Debug.LogWarning("Its not your turn!");
            card.GetComponent<CardClicker>().played = false;
            return;
        }

        if(!players[me].limit.Permits(card))
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
                    //skipButton.SetActive(false);
                    players[turn].myTurn = false;
                    card.GetComponent<CardClicker>().played = true;
                    PlayCreature(c.Type, me);
                    //Networking.server.playCard();
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
                //skipButton.SetActive(false);
                cardPlayed_playerTarget = card;
                EnableTargetButtons(true);
                break;
            case Magic m when m.target == MagicTarget.All:
                //skipButton.SetActive(false);
                players[turn].myTurn = false;
                card.GetComponent<CardClicker>().played = true;
                //Networking.server.playCard(card.GetComponent<CardData>().cardName);
                break;
            case Magic m when m.target == MagicTarget.Opponent:
                //skipButton.SetActive(false);
                cardPlayed_playerTarget = card;
                EnableTargetButtons(true);
                break;
            case Magic m when m.target == MagicTarget.Card:
                cardPlayed_otherCreature = card;
                targetBox.SetActive(true);
                break;
        }
    }

    public void SkipTurn()
    {
        if (turn == me && players[me].myTurn)
        {
            players[turn].myTurn = false;
            //skipButton.SetActive(false);
            //Networking.server.playCard("none");
        }
    }

    void EnableTargetButtons(bool enable)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (!players[i].disconnected) playerTargetButtons[i].SetActive(enable);
        }
    }

    public void ChoosePlayer(int index)
    {
        players[turn].myTurn = false;
        cardPlayed_playerTarget.GetComponent<CardClicker>().played = true;
        //Networking.server.playCard(cardPlayed_playerTarget.GetComponent<CardData>().cardName, players[index].id);

        EnableTargetButtons(false);
        cardPlayed_playerTarget = null;
    }

    public void SubtractCreature(CreatureType type, int index)
    {
        players[index].creatureAmounts[type]--;
    }
}

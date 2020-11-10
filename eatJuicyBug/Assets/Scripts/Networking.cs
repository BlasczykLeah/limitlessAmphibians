using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

public class Networking : MonoBehaviour
{
    [TextArea]
    public string serverQuickRef;

    public static Networking server;
    SocketIOComponent socket;

    char quote = '"';

    public List<string> playerSockets;
    string mySocket = "";
    int myPlayerIndex = -1;

    public bool host;

    public string winner = "";

    SocketIOEvent tempDataStore;

    private void Awake()
    {
        if (server) Destroy(gameObject);
        else server = this;

        DontDestroyOnLoad(gameObject);
        playerSockets = new List<string>();

        socket = GetComponent<SocketIOComponent>();
    }

    void Start()
    {
        socket.On("connectionmessage", onConnectionEstabilished);
        socket.On("users", loadUsers);
        socket.On("removeUser", removeUser);
        socket.On("allUsers", allUsers);
        socket.On("loadGameScene", gameScene);
        socket.On("cardPlayed", recieveCardPlayed);
        socket.On("drewCard", recieveCardDrawn);
        socket.On("newHand", newHand);
        socket.On("host", setHost);
        socket.On("playerTurn", getWhosTurn);
        socket.On("playerValues", setPlayerCards);
        socket.On("cardRemoved", checkForRemove);
        socket.On("winscene", goToWin);
    }

    #region Starting Game

    void onConnectionEstabilished(SocketIOEvent evt)
    {
        Debug.Log("Player is connected: " + evt.data.GetField("id"));
        if (mySocket == "") mySocket = evt.data.GetField("id").ToString().Trim('"');
    }

    void setHost(SocketIOEvent evt)
    {
        Debug.Log("You are now the host.");
        //host = true;
    }

    public void newUsername(string name)
    {
        name = quote + name + quote;

        JSONObject nameUpload = new JSONObject(name);
        socket.Emit("updateUsername", nameUpload);
    }

    void loadUsers(SocketIOEvent evt)
    {
        Debug.Log("loading usernames...");

        for (int i = 0; i < evt.data.Count; i++)
        {
            JSONObject jsonData = evt.data.GetField(i.ToString());

            Debug.Log(jsonData.GetField("username"));
            Usernames.inst.addUsername(jsonData.GetField("id").ToString().Trim('"'), jsonData.GetField("username").ToString().Trim('"'));

            if (!playerSockets.Contains(jsonData.GetField("id").ToString().Trim('"')))
                playerSockets.Add(jsonData.GetField("id").ToString().Trim('"'));
        }
    }

    void removeUser(SocketIOEvent evt)
    {
        if (Usernames.inst) Usernames.inst.removeUsername(evt.data.GetField("id").ToString().Trim('"'));
        else
        {
            int playerGone = GameManager.instance.GetPlayerIndexFromID(evt.data.GetField("id").ToString().Trim('"'));
            FindObjectOfType<PlayerViews>().viewButtons[playerGone].SetActive(false);
            GameManager.instance.players[playerGone].disconnected = true;
        }
        playerSockets.Remove(evt.data.GetField("id").ToString().Trim('"'));
    }

    private void OnApplicationQuit()
    {
        if(GameManager.instance is null)
        {
            Debug.Log("GameManager instance is null");
            return;
        }

        foreach(GameObject c in GameManager.instance.playerHand.GetComponentsInChildren<GameObject>())
        {
            socket.Emit("discardCard", new JSONObject(quote + c.GetComponent<CardData>().cardName + quote));
        }

        socket.Emit("discardCard", new JSONObject(quote + GameManager.instance.players[myPlayerIndex].limit.GetComponent<CardData>().cardName + quote));
    }

    public void getPlayers()
    {
        socket.Emit("getUsernames");
    }

    void allUsers(SocketIOEvent evt)
    {
        // has all users, used for setting up who is which indexed player

        GameManager.instance.players = new List<Player>();

        for (int i = 0; i < evt.data.Count; i++)
        {
            JSONObject jsonData = evt.data.GetField(i.ToString());

            Player thing = new Player();
            thing.name = jsonData.GetField("username").ToString().Trim('"');
            thing.id = jsonData.GetField("id").ToString().Trim('"');

            if (thing.id == mySocket) myPlayerIndex = i;

            GameManager.instance.players.Add(thing);
        }

        Debug.Log("Players loaded successfully");

        if (host)
        {
            Debug.Log("loading cards...");
            socket.Emit("loadHands");
        }

        //GameManager.instance.ready = true;
    }

    public void loadGame()
    {
        host = true;
        socket.Emit("loadGame");
    }

    void gameScene(SocketIOEvent evt)
    {
        Debug.Log("Attempting to load game scene");
        SceneManager.LoadScene(1);

        socket.Emit("loadingHands");
    }

    #endregion

    #region Game Functions

    void newHand(SocketIOEvent evt)
    {
        if (myPlayerIndex != -1)
        {
            GameObject limitCard;
            GameObject winCard;
            GameObject[] newHand = new GameObject[3];

            //GameManager.instance.players[myPlayerIndex].Hand.Add(CardDictionary.instance.GetCard(evt.data.GetField("card1").ToString().Trim('"')).GetComponent<Card>());
            newHand[0] = CardDictionary.instance.GetCard(evt.data.GetField("card1").ToString().Trim('"'));
            newHand[1] = CardDictionary.instance.GetCard(evt.data.GetField("card2").ToString().Trim('"'));
            newHand[2] = CardDictionary.instance.GetCard(evt.data.GetField("card3").ToString().Trim('"'));

            limitCard = CardDictionary.instance.GetCard(evt.data.GetField("limit").ToString().Trim('"'));

            winCard = CardDictionary.instance.GetCard(evt.data.GetField("win").ToString().Trim('"'));

            Debug.Log("Hand created: " + newHand[0].name + ", " + newHand[1].name + ", " + newHand[2].name);
            GameManager.instance.InstantiateMyCards(myPlayerIndex, newHand, limitCard, winCard);
        }
        else
        {
            Debug.LogError("Player instance not saved. Cannot add cards, trying again in 0.5s");
            tempDataStore = evt;
            Invoke("tryNewHand", 0.5F);
            return;

        }

        GameManager.instance.ready = true;
        if (host) socket.Emit("firstTurn");

        string thing = "{ " + quote + "limit" + quote + ":" + evt.data.GetField("limit").ToString() + ", " + quote + "wincon" + quote + ":" + evt.data.GetField("win").ToString() + " }";
        socket.Emit("setValues", new JSONObject(thing));
    }

    void tryNewHand()
    {
        if (myPlayerIndex != -1)
        {
            GameObject limitCard;
            GameObject winCard;
            GameObject[] newHand = new GameObject[3];

            //GameManager.instance.players[myPlayerIndex].Hand.Add(CardDictionary.instance.GetCard(evt.data.GetField("card1").ToString().Trim('"')).GetComponent<Card>());
            newHand[0] = CardDictionary.instance.GetCard(tempDataStore.data.GetField("card1").ToString().Trim('"'));
            newHand[1] = CardDictionary.instance.GetCard(tempDataStore.data.GetField("card2").ToString().Trim('"'));
            newHand[2] = CardDictionary.instance.GetCard(tempDataStore.data.GetField("card3").ToString().Trim('"'));

            limitCard = CardDictionary.instance.GetCard(tempDataStore.data.GetField("limit").ToString().Trim('"'));

            winCard = CardDictionary.instance.GetCard(tempDataStore.data.GetField("win").ToString().Trim('"'));

            Debug.Log("Hand created: " + newHand[0].name + ", " + newHand[1].name + ", " + newHand[2].name);
            GameManager.instance.InstantiateMyCards(myPlayerIndex, newHand, limitCard, winCard);
        }
        else
        {
            Debug.LogError("Player instance not saved. Cannot add cards, trying again in 0.5s");
            Invoke("tryNewHand", 0.5F);
            return;
        }

        GameManager.instance.ready = true;
        if (host) socket.Emit("firstTurn");

        string thing = "{ " + quote + "limit" + quote + ":" + tempDataStore.data.GetField("limit").ToString() + ", " + quote + "wincon" + quote + ":" + tempDataStore.data.GetField("win").ToString() + " }";
        socket.Emit("setValues", new JSONObject(thing));
        tempDataStore = null;
    }

    void recieveCardPlayed(SocketIOEvent evt)
    {
        string cardString = evt.data.GetField("card").ToString().Trim('"');
        string playerID = evt.data.GetField("id").ToString().Trim('"');

        int playerIndex = GameManager.instance.GetPlayerIndexFromID(playerID);

        string targetPlayer = evt.data.GetField("targetID").ToString().Trim('"');
        int targetPlayerIndex = -1;
        if (targetPlayer != "none") targetPlayerIndex = GameManager.instance.GetPlayerIndexFromID(targetPlayer);
        string targetCard = evt.data.GetField("targetCard").ToString().Trim('"');

        // **Assuming creature card for now

        Debug.Log(GameManager.instance.players[playerIndex].name + " has played " + cardString);

        if (cardString != "none")
        {
            GameObject cardPlayed = null;

            if (myPlayerIndex == playerIndex)
            {
                for (int i = 0; i < GameManager.instance.playerHand.transform.childCount; i++)
                {
                    if (GameManager.instance.playerHand.transform.GetChild(i).GetComponent<CardData>().cardName == cardString)
                    {
                        cardPlayed = GameManager.instance.playerHand.transform.GetChild(i).gameObject;

                        cardPlayed.transform.SetParent(null);
                        cardPlayed.transform.localScale = Vector3.one * 0.2F;
                        cardPlayed.transform.rotation = Quaternion.Euler(Vector3.right * 90F);

                        GameManager.instance.players[playerIndex].Hand.Remove(cardPlayed.GetComponent<Card>());
                        continue;
                    }
                }
            }
            else
            {
                //CreatureType creature = CardDictionary.instance.GetCard(cardString).GetComponent<CardData>().GetCreatureType();
                cardPlayed = Instantiate(CardDictionary.instance.GetCard(cardString));
                cardPlayed.GetComponent<CardClicker>().played = true;
            }

            if (cardPlayed)
            {
                if (cardPlayed.GetComponent<CardData>().GetCardType() == CardType.Creature)
                {
                    if(targetCard != "none")    // card was removed to play
                    {
                        GameManager.instance.tableLayouts[playerIndex].removePlacedCard(targetCard);
                        GameManager.instance.players[playerIndex].cardsOnTable--;
                    }

                    cardPlayed.GetComponent<CardData>().playerIndex = playerIndex;
                    GameManager.instance.tableLayouts[playerIndex].placeCard(cardPlayed);
                    GameManager.instance.players[playerIndex].cardsOnTable++;
                    GameManager.instance.PlayCreature(cardPlayed.GetComponent<CardData>().GetCreatureType(), playerIndex);
                }
                else if (cardPlayed.GetComponent<CardData>().GetCardType() == CardType.Limit)
                {
                    GameManager.instance.PlayLimit(cardPlayed.GetComponent<CardData>().limit, targetPlayerIndex);
                }
                else // is a magic card
                {
                    if(targetPlayerIndex == -1)
                    {
                        // has no player target
                        CreatureType creatureType = cardPlayed.GetComponent<CardData>().GetCreatureType();
                        cardPlayed.GetComponent<CardData>().magic.DoMagic(playerIndex, targetPlayerIndex, creatureType);
                    }
                    else if (targetCard != "none")
                    {
                        GameObject targettedCard = null;
                        foreach (GameObject card in GameManager.instance.tableLayouts[targetPlayerIndex].tableCards)
                        {
                            if (card.GetComponent<CardData>().cardName == targetCard)
                            {
                                targettedCard = card;
                                continue;
                            }
                        }

                        if (targettedCard)
                        {
                            // PLAY MAGIC CARD THAT TARGETS ANOTHER PLAYERS CARD
                            CreatureType creatureType = targettedCard.GetComponent<CardData>().GetCreatureType();
                            Debug.Log("==================");
                            Debug.Log("user: " + playerIndex);
                            Debug.Log("target: " + targetPlayerIndex);
                            Debug.Log("creature: " + creatureType.ToString());
                            Debug.Log("==================");
                            cardPlayed.GetComponent<CardData>().magic.DoMagic(playerIndex, targetPlayerIndex, creatureType);
                        }
                        else Debug.LogError("target card not found.");

                        Destroy(cardPlayed);
                    }

                    // PLAY MAGIC CARD THAT DOES NOT TARGET A CARD
                    CreatureType type = cardPlayed.GetComponent<CardData>().GetCreatureType();
                    Debug.Log("==================");
                    Debug.Log("user: " + playerIndex);
                    Debug.Log("target: " + targetPlayerIndex);
                    Debug.Log("creature: " + type.ToString());
                    Debug.Log("==================");
                    cardPlayed.GetComponent<CardData>().magic.DoMagic(playerIndex, targetPlayerIndex, type);
                }
            }
            else Debug.LogError("D:");
        }

        if (host)
        {
            Debug.Log("I am setting the next person's turn");
            Invoke("EnableNextTurn", 3F);
        }
        GameManager.instance.NextTurn();
    }

    void recieveCardDrawn(SocketIOEvent evt)
    {
        string cardString = evt.data.GetField("card").ToString().Trim('"');
        GameObject newCard = CardDictionary.instance.GetCard(cardString);
        GameManager.instance.DrawCard(newCard);
    }

    public void playCard(string cardName)
    {
        string thing = "{ " + quote + "card" + quote + ":" + quote + cardName + quote + ", " + quote + "targetID" + quote + ":" + quote + "none" + quote + ", " + quote + "targetCard" + quote + ":" + quote + "none" + quote + " }";
        socket.Emit("playCard", new JSONObject(thing));
    }

    public void playCard(string cardName, string playerTarget)
    {
        string thing = "{ " + quote + "card" + quote + ":" + quote + cardName + quote + ", " + quote + "targetID" + quote + ":" + quote + playerTarget + quote + ", " + quote + "targetCard" + quote + ":" + quote + "none" + quote + " }";
        socket.Emit("playCard", new JSONObject(thing));
    }

    public void playCard(string cardName, string playerTarget, string cardSelected)
    {
        string thing = "{ " + quote + "card" + quote + ":" + quote + cardName + quote + ", " + quote + "targetID" + quote + ":" + quote + playerTarget + quote + ", " + quote + "targetCard" + quote + ":" + quote + cardSelected + quote + " }";
        socket.Emit("playCard", new JSONObject(thing));
    }

    public void drawCard()
    {
        socket.Emit("drawCard");
    }

    public void discardCard(string cardName)
    {
        socket.Emit("discardCard", new JSONObject(quote + cardName + quote));
    }

    void checkForRemove(SocketIOEvent evt)
    {
        string cardString = evt.data.GetField("card").ToString().Trim('"');
        string playerID = evt.data.GetField("id").ToString().Trim('"');
        int playerIndex = GameManager.instance.GetPlayerIndexFromID(playerID);

        if (CardDictionary.instance.GetCard(cardString).GetComponent<CardData>().GetCardType() == CardType.Creature)
        {
            GameManager.instance.tableLayouts[playerIndex].removePlacedCard(cardString);
            GameManager.instance.players[playerIndex].cardsOnTable--;
        }
    }

    void getWhosTurn(SocketIOEvent evt)
    {
        string playerID = evt.data.GetField("id").ToString().Trim('"');
        int index = GameManager.instance.GetPlayerIndexFromID(playerID);
        Debug.Log(GameManager.instance.players[index].name + "'s turn! - says the server");
        GameManager.instance.setTurn(index);
    }

    public void EnableNextTurn()
    {
        socket.Emit("nextTurn");
    }

    void setPlayerCards(SocketIOEvent evt)
    {
        string newWinStr = evt.data.GetField("win").ToString().Trim('"');
        string newLimStr = evt.data.GetField("limit").ToString().Trim('"');

        string playerID = evt.data.GetField("id").ToString().Trim('"');
        int playerIndex = GameManager.instance.GetPlayerIndexFromID(playerID);

        if(newWinStr != "none")
        {
            GameObject winCard = CardDictionary.instance.GetCard(newWinStr);
            GameObject newWin = Instantiate(winCard);
            newWin.GetComponent<CardClicker>().played = true;
            GameManager.instance.tableLayouts[playerIndex].addWinCard(newWin);
            GameManager.instance.players[playerIndex].winCon = newWin.GetComponent<WinCondition>();
        }
        if(newLimStr != "none")
        {
            GameObject limitCard = CardDictionary.instance.GetCard(newLimStr);
            GameObject newLimit = Instantiate(limitCard);
            newLimit.GetComponent<CardClicker>().played = true;
            GameManager.instance.tableLayouts[playerIndex].addLimitCard(newLimit);
        }
    }

    public void updateValues(string limit, string win)
    {
        string thing = "{ " + quote + "limit" + quote + ":" + quote + limit + quote + ", " + quote + "wincon" + quote + ":" + quote + win + quote + " }";
        socket.Emit("setValues", new JSONObject(thing));
    }

    public void sendWin()
    {
        socket.Emit("win", new JSONObject(quote + GameManager.instance.players[myPlayerIndex].id + quote));
    }

    void goToWin(SocketIOEvent evt)
    {
        // go to win screen and update win text
        string winnerID = evt.data.GetField("id").ToString().Trim('"');
        int playerIndex = GameManager.instance.GetPlayerIndexFromID(winnerID);
        winner = GameManager.instance.players[playerIndex].name;

        Debug.Log("Loading Win Scene");
        SceneManager.LoadScene(2);
    }

    #endregion
}
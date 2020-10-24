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
    string mySocket;
    Player myPlayer;

    public bool isHost = false;

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
        socket.On("host", setAsHost);
    }

    #region Starting Game

    void onConnectionEstabilished(SocketIOEvent evt)
    {
        Debug.Log("Player is connected: " + evt.data.GetField("id"));
        if (mySocket == "") mySocket = evt.data.GetField("id").ToString().Trim('"');
    }

    void setAsHost(SocketIOEvent evt)
    {
        isHost = true;
        Debug.Log("You are set as the host of this game.");
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
        Usernames.inst.removeUsername(evt.data.GetField("id").ToString().Trim('"'));
        playerSockets.Remove(evt.data.GetField("id").ToString().Trim('"'));
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

            if (thing.id == mySocket) myPlayer = thing;

            GameManager.instance.players.Add(thing);
        }

        if (isHost) socket.Emit("loadingHands");
        else Debug.LogWarning("You are not the host. Host is dealing cards...");

        //GameManager.instance.ready = true;
    }

    public void loadGame()
    {
        socket.Emit("loadGame");
    }

    void gameScene(SocketIOEvent evt)
    {
        Debug.Log("Attempting to load game scene");
        SceneManager.LoadScene(1);
    }

    #endregion

    #region Game Functions

    void newHand(SocketIOEvent evt)
    {
        if (myPlayer != null)
        {
            myPlayer.Hand.Add(CardDictionary.instance.GetCard(evt.data.GetField("card1").ToString().Trim('"')).GetComponent<Card>());
            myPlayer.Hand.Add(CardDictionary.instance.GetCard(evt.data.GetField("card2").ToString().Trim('"')).GetComponent<Card>());
            myPlayer.Hand.Add(CardDictionary.instance.GetCard(evt.data.GetField("card3").ToString().Trim('"')).GetComponent<Card>());
        }
        else Debug.LogError("Player instance not saved. Cannot add cards.");

        GameManager.instance.ready = true;
    }

    void recieveCardPlayed(SocketIOEvent evt)
    {
        string cardString = evt.data.GetField("card").ToString().Trim('"');
        string playerID = evt.data.GetField("id").ToString().Trim('"');
    }

    void recieveCardDrawn(SocketIOEvent evt)
    {
        string cardString = evt.data.GetField("card").ToString().Trim('"');
    }

    public void playCard(string cardName)
    {
        socket.Emit("playCard", new JSONObject(quote + cardName + quote));
    }

    public void drawCard()
    {
        socket.Emit("drawCard");
    }

    public void discardCard(string cardName)
    {
        socket.Emit("discardCard", new JSONObject(quote + cardName + quote));
    }

    #endregion
}
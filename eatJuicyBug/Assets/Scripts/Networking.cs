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
    }

    // This is the listener function definition
    void onConnectionEstabilished(SocketIOEvent evt)
    {
        Debug.Log("Player is connected: " + evt.data.GetField("id"));
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

            GameManager.instance.players.Add(thing);
        }
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
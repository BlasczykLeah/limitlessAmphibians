using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SocialPlatforms.Impl;

public class Networking : MonoBehaviour
{
    public static Networking inst;

    SocketIOComponent socket;
    //HighScoreFunctions hsf;

    int highScoreLoadIndex = 0;
    List<HighScore> highScores;

    private void Awake()
    {
        if (inst) Destroy(gameObject);
        else inst = this;

        DontDestroyOnLoad(gameObject);

        socket = GetComponent<SocketIOComponent>();
        socket.Connect();
    }

    void Start()
    {
        socket.On("connectionmessage", onConnectionEstabilished);
        socket.On("LoadScore", LoadScore);
        socket.On("ScoresUpdated", SendToUI);
    }

    // This is the listener function definition
    void onConnectionEstabilished(SocketIOEvent evt)
    {
        Debug.Log("Player is connected: " + evt.data.GetField("id"));
        SendScore("coolname", 3.2345F);
    }

    public void button(int num)
    {
        JSONObject test = new JSONObject(num);
        socket.Emit("buttonClicked", test);
    }

    public void LoadScore(SocketIOEvent evt)
    {
        string newName = evt.data.GetField("name").ToString();
        newName.Trim('"');

        if (newName != "" && newName != "undefined")
        {
            string tempScore = evt.data.GetField("score").ToString();
            tempScore.Trim('"');

            Debug.Log("Adding " + newName + ", " + tempScore);

            float newScore;
            float.TryParse(tempScore, out newScore);

            HighScore newHS = new HighScore(newName, newScore);
            highScores.Add(newHS);
        }
    }

    public void SendScore(string name, float score)
    {
        char quote = '"';

        string thing = "{ " + quote + "name" + quote + ":" + quote + name + quote + ", " + quote + "score" + quote + ":" + score + " }";
        JSONObject send = new JSONObject(thing);

        socket.Emit("EnterScore", send);

        //Invoke("LoadScores", 2F);
    }

    public void LoadScores()
    {
        //hsf = sendTo;

        highScoreLoadIndex = 0;
        highScores = new List<HighScore>();

        for(int i = 0; i < 5; i++)
        {
            JSONObject send = new JSONObject(highScoreLoadIndex + 1);
            socket.Emit("LoadTheScore", send);
            highScoreLoadIndex++;
        }

        Debug.Log("Scores loaded.");

        //StartCoroutine("SendToUI", sendTo);
    }

    void SendToUI(SocketIOEvent evt)
    {
        if (highScores.Count > 0)
        {
            for (int i = 0; i < highScores.Count; i++)
            {
                Debug.Log(highScores[i].name + " got a score of " + highScores[i].score);
            }

            //hsf.ReceiveScores(highScores);
        }
        else Debug.LogError("no highscores found");
    }

    public void ResetHighScores()
    {
        socket.Emit("ResetScores");
        Debug.Log("High scores have been reset.");
    }
}


public class HighScore
{
    public string name;
    public float score;

    public HighScore(string n, float f)
    {
        name = n;
        score = f;
    }
}
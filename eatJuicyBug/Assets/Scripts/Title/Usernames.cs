﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Usernames : MonoBehaviour
{
    public static Usernames inst;

    public TMP_InputField nameField;

    public GameObject usernameList;
    public GameObject textPrefab;
    public GameObject playButton;

    Dictionary<string, GameObject> usernameTexts;

    private void Start()
    {
        if (inst) Destroy(gameObject);
        else inst = this;

        usernameTexts = new Dictionary<string, GameObject>();

        if (PlayerPrefs.HasKey("username"))
        {
            nameField.placeholder.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("username");
            nameField.text = nameField.placeholder.GetComponent<TextMeshProUGUI>().text;
            Invoke("acceptName", 0.15F);
        }
    }

    public void acceptName()
    {
        if (nameField.text != "")
        {
            PlayerPrefs.SetString("username", nameField.text);
            Networking.server.newUsername(nameField.text);
    
            // maybe sets username here
            nameField.placeholder.GetComponent<TextMeshProUGUI>().text = nameField.text;
            nameField.text = "";
        }
    }

    public void addUsername(string id, string name)
    {
        if (usernameTexts.ContainsKey(id))
        {
            usernameTexts[id].GetComponent<TextMeshProUGUI>().text = name;
        }
        else
        {
            GameObject newText = Instantiate(textPrefab, usernameList.transform);
            newText.GetComponent<TextMeshProUGUI>().text = name;
            usernameTexts.Add(id, newText);
        }

        if (usernameTexts.Count > 1) playButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        else playButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void removeUsername(string id)
    {
        if (usernameTexts.ContainsKey(id))
        {
            GameObject remove = usernameTexts[id];
            usernameTexts.Remove(id);
            Destroy(remove);

            if (usernameTexts.Count < 2) playButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            Debug.LogError("user not found, could not remove from list");
        }
    }

    public void playGame()
    {
        Debug.Log("starting game...");
        Networking.server.loadGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerViews : MonoBehaviour
{
    public GameObject[] viewButtons;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void enableButtons(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            viewButtons[i].SetActive(true);
            viewButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.instance.players[i].name;
        }
    }

    public void setView(int index)
    {
        Camera.main.transform.position = new Vector3(index * 30, 10, 0);
    }
}

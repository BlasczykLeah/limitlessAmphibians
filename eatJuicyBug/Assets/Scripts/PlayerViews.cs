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
            viewButtons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.instance.players[i].name;
        }
    }

    public void setView(int index)
    {
        resetAllHighlights();
        viewButtons[index].transform.GetChild(0).gameObject.SetActive(true);
        Camera.main.transform.position = new Vector3(index * 30, 10, 0);
    }

    void resetAllHighlights()
    {
        foreach(GameObject a in viewButtons)
        {
            a.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}

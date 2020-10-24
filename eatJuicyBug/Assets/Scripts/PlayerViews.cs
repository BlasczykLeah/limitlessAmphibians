using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (int i = 0; i < amount; i++) viewButtons[i].SetActive(true);
    }

    public void setView(int index)
    {
        Camera.main.transform.position = new Vector3(index * 30, 10, 0);
    }
}

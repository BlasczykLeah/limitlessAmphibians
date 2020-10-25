using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winnerText : MonoBehaviour
{
    public Text text;
    public string winnerName;
    // Start is called before the first frame update
    void Start()
    {
        text.text = winnerName + " is \nthe champion";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

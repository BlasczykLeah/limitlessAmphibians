using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class loadWin : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        string winner = Networking.server.winner;
        text.text = winner + " is the champion";
    }
}

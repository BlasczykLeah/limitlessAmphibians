using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public List<Card> Hand = new List<Card>();
    public WinCondition winCon;
    public Limit limit;
    public string id;
}
